using GaVL.Application.Auths;
using GaVL.Application.Catalog.Mods;
using GaVL.Application.Systems;
using GaVL.DTO.Settings;
using GaVL.Utilities;

namespace GaVL.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddService(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<TurnstileSetting>(config.GetSection(SystemConstant.TURNSTILE));
            services.Configure<JwtSettings>(config.GetSection(SystemConstant.JWT_SETTING));
            services.Configure<AppUrlSetting>(config.GetSection(SystemConstant.APP_URL_SETTING));
            services.AddHttpClient();
            return services;
        } 
        public static IServiceCollection AddDIService(this IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ITurnstileService, TurnstileService>();
            services.AddScoped<IModService, ModService>();
            return services;
        }
    }
}
