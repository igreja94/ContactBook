using Application.Mappers;
using Application.Mappers.Interfaces;
using Application.Services;
using Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddSingleton<IUserMapper, UserMapper>();
            services.AddSingleton<IContactMapper, ContactMapper>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IContactService, ContactService>();

            return services;
        }
    }
}
