using GaVL.Utilities;
using Microsoft.Extensions.Options;
using System.Threading.RateLimiting;

namespace GaVL.API.Extensions
{
    public static class LoginExtensions
    {
        public static IServiceCollection ConfigureRateLimited(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.AddPolicy(SystemConstant.POLICY_LOGIN_IP, context =>
                {
                    string ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                    if (context.Request.Headers.ContainsKey("X-Forwarded-For"))
                    {
                        ipAddress = context.Request.Headers["X-Forwarded-For"].ToString().Split(',')[0].Trim();
                    }
                    else if (context.Request.Headers.ContainsKey("X-Real-IP"))
                    {
                        ipAddress = context.Request.Headers["X-Real-IP"].ToString();
                    }
                    if (!string.IsNullOrEmpty(ipAddress) && ipAddress.Contains(':'))
                    {
                        ipAddress = ipAddress.Split(':')[0];
                    }
                    return RateLimitPartition.GetFixedWindowLimiter(ipAddress, _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 5,
                        Window = TimeSpan.FromMinutes(5),
                        QueueLimit = 0
                    });
                });
            });
            return services;
        }
    }
}
