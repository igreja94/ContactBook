using Application.DTO;
using Application.Services;
using Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IService<UserDto>, UserService>();
            services.AddScoped<IContactService, ContactService>();

            return services;
        }
    }
}
