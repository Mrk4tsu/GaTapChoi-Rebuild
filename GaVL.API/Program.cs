using GaVL.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureDbContext(builder.Configuration)
                .ConfigureRedis(builder.Configuration)
                .AddAuthorization()
                .AddHttpContextAccessor()
                .AddDIService()
                .ConfigureJwt(builder.Configuration)
                .AddSmtpConfig(builder.Configuration)
                .AddService(builder.Configuration)
                .AddSwaggerExplorer();

builder.Services.AddControllers();

var app = builder.Build();

app.ConfigureSwaggerExplorer()
   .ConfigureCORS()
   .AddIdentityAuthMiddlewares();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
