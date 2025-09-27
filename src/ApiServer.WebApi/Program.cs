using ApiServer.Application;
using ApiServer.Infrastructure;
using ApiServer.WebApi.Extensions;
using ApiServer.WebApi.Middlewares;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// 配置 Serilog
builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day);
});

// Add services to the container.
builder.Services.AddControllers();

// 添加静态文件服务
builder.Services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = "wwwroot";
});

// 添加应用层服务
builder.Services.AddApplication();

// 添加基础设施层服务
builder.Services.AddInfrastructure(builder.Configuration);

// 添加Web API服务
builder.Services.AddWebApiServices(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    // 确保数据库已创建
    await app.Services.EnsureDatabaseCreatedAsync();
}

// 配置中间件管道
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<RequestResponseLoggingMiddleware>();

app.UseHttpsRedirection();

// 启用CORS
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// 配置静态文件和SPA
app.UseStaticFiles();
app.UseSpaStaticFiles();

// 配置SPA
app.UseSpa(spa =>
{
    spa.Options.SourcePath = "../frontend";
    
    if (app.Environment.IsDevelopment())
    {
        spa.UseProxyToSpaDevelopmentServer("http://localhost:8080");
        spa.UseProxyToSpaDevelopmentServer("http://localhost:8081");
        spa.UseProxyToSpaDevelopmentServer("http://localhost:8082");
        spa.UseProxyToSpaDevelopmentServer("http://localhost:8083");
    }
});

Log.Information("Application starting up");

try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}