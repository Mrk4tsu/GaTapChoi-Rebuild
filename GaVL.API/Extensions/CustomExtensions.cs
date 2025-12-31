using GaVL.API.Hubs;
using GaVL.Application.Auths;
using GaVL.Application.Systems;
using GaVL.Data;
using GaVL.DTO.Settings;
using GaVL.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System.Text;

namespace GaVL.API.Extensions
{
    public static class CustomExtensions
    {
        public static IServiceCollection ConfigureDbContext(this IServiceCollection services, IConfiguration config, IHostEnvironment env)  // Thêm IHostEnvironment để check env
        {
            var connectionString = config.GetConnectionString(SystemConstant.DB_CONNECTION_STRING);
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString), "Database connection string is missing.");

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorCodesToAdd: null);

                    npgsqlOptions.CommandTimeout(30);
                    npgsqlOptions.ProvideClientCertificatesCallback(cert => {
                    });
                });

                if (env.IsDevelopment())
                {
                    options.EnableSensitiveDataLogging();
                    options.EnableDetailedErrors();
                }
                else
                {
                    options.LogTo(Console.WriteLine, LogLevel.Warning);
                }
            });

            return services;
        }
        public static IServiceCollection ConfigureRedis(this IServiceCollection services, IConfiguration config)
        {
            var connectionStringRedis = config.GetConnectionString(SystemConstant.REDIS_CONNECTION_STRING);
            if (string.IsNullOrEmpty(connectionStringRedis))
                throw new ArgumentNullException(nameof(connectionStringRedis), "Redis connection string is missing.");
            services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(connectionStringRedis));

            services.AddScoped<IRedisService, RedisService>();
            return services;
        }
        public static IServiceCollection AddSwaggerExplorer(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Swagger MrKatsuWeb Solution", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                        Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
            });
            return services;
        }
        public static IApplicationBuilder ConfigureSwaggerExplorer(this IApplicationBuilder app)
        {
            var ev = app.ApplicationServices.GetRequiredService<IHostEnvironment>();
            if (ev.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            return app;
        }
        public static IServiceCollection AddSmtpConfig(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<MailJetSetting>(config.GetSection(SystemConstant.SMTP_SETTINGS));
            services.AddSingleton<IMailService, MailService>();
            return services;
        }
        public static IApplicationBuilder ConfigureCORS(this IApplicationBuilder app)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            var allowedOrigins = new string[]
            {
                "http://localhost:4200",
                "https://admin.gavl.io.vn",
                "https://gavl.io.vn",
                "https://forum.gavl.io.vn",
                "http://127.0.0.1:5500",
                "http://127.0.0.1:8787",
            };

            app.UseCors(options =>
                options.WithOrigins(allowedOrigins)
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials());

            return app;
        }
        public static IServiceCollection ConfigureJwt(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<JwtSettings>(config.GetSection(SystemConstant.JWT_SETTING));
            services.AddScoped<TokenService>();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = config["JwtSettings:Issuer"],
                    ValidAudience = config["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JwtSettings:SecretKey"]!))
                };
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var redis = context.HttpContext.RequestServices.GetRequiredService<IRedisService>();
                        var token = context.SecurityToken as JsonWebToken;
                        if (token == null)
                        {
                            context.Fail("Invalid token type.");
                            return;
                        }
                        var blacklistKey = $"blacklist:{token.EncodedToken}";
                        if (await redis.KeyExist(blacklistKey))
                        {
                            context.Fail("Token has been revoked.");
                        }
                    }
                };
            });

            return services;
        }
        public static IApplicationBuilder AddIdentityAuthMiddlewares(this IApplicationBuilder app)
        {
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            return app;
        }
        public static IApplicationBuilder AddSignalRHubMiddlewares(this IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<NotifyHub>("notify");
            });
            return app;
        }
    }
}
