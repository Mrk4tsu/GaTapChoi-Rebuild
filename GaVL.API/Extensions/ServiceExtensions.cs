using GaVL.Application.Auths;
using GaVL.Application.Systems;
using GaVL.DTO.Settings;
using GaVL.Utilities;

namespace GaVL.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddService(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<JwtSettings>(config.GetSection(SystemConstant.JWT_SETTING));
            return services;
        } 
        public static IServiceCollection AddDIService(this IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenService, TokenService>();
            return services;
        }
    }
}
