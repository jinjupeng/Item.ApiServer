using ApiServer.Application;
using ApiServer.Infrastructure;
using ApiServer.Infrastructure.Extensions;
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
}

// 自动执行数据库迁移和数据初始化
try
{
    Log.Information("程序启动时自动执行数据库迁移和初始化...");
    await app.Services.AutoMigrateAndInitializeAsync();
    Log.Information("数据库迁移和初始化完成，程序继续启动");
}
catch (Exception ex)
{
    Log.Fatal(ex, "数据库迁移或初始化失败，程序启动终止");
    throw;
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