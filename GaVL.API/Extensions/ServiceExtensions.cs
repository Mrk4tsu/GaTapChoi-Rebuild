using GaVL.Application.Auths;

namespace GaVL.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddDIService(this IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddScoped<IAuthService, AuthService>();
            return services;
        }
    }
}
