using Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Webhost.ExceptionHandling
{
    public sealed class ApiExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
    {
        private const int SqlServerForeignKeyViolation = 547;

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            ProblemDetails? problem = exception switch
            {
                NotFoundException => new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Not Found",
                    Detail = exception.Message,
                },
                ValidationFailedException validation => new ValidationProblemDetails(
                    new Dictionary<string, string[]> { [validation.PropertyName] = [validation.Message] })
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "One or more validation errors occurred.",
                },
                DbUpdateException { InnerException: SqlException { Number: SqlServerForeignKeyViolation } } => new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "Conflict",
                    Detail = "The request conflicts with related data, for example the referenced user no longer exists.",
                },
                _ => null,
            };

            if (problem is null)
            {
                return false;
            }

            httpContext.Response.StatusCode = problem.Status!.Value;

            return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = problem,
            });
        }
    }
}
