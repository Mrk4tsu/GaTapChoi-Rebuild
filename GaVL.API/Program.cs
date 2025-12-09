using GaVL.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureDbContext(builder.Configuration)
                .ConfigureRedis(builder.Configuration)
                .AddAuthorization()
                .AddHttpContextAccessor()
                .AddDIService()
                .AddService(builder.Configuration)
                .AddSwaggerExplorer();

builder.Services.AddControllers();

var app = builder.Build();

app.ConfigureSwaggerExplorer();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
