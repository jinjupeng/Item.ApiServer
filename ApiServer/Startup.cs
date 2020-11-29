using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using System.Threading.Tasks;
using ApiServer.Exception;
using Microsoft.Extensions.Hosting;
using System.IO;
using Microsoft.OpenApi.Models;
using ApiServer.JWT;

namespace ApiServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // 使用DI将服务注入到容器中
        public void ConfigureServices(IServiceCollection services)
        {
            #region JwtSetting类注入
            services.Configure<JwtSettings>(Configuration.GetSection("JwtSettings"));
            JwtSettings setting = new JwtSettings();
            Configuration.Bind("JwtSettings", setting);
            JwtHelper.Settings = setting;
            #endregion

            #region 基于策略模式的授权
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Permission", policy => policy.Requirements.Add(new PermissionRequirement()));

            })

            #region JWT认证，core自带官方jwt认证
            // 开启Bearer认证
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            // 添加JwtBearer服务：
            .AddJwtBearer(config =>
            {
                config.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,// 是否验证Issuer
                    ValidateAudience = true,// 是否验证Audience
                    ValidateLifetime = true,// 是否验证失效时间
                    ValidateIssuerSigningKey = true,// 是否验证SecurityKey
                    ValidAudience = setting.Audience,// Audience
                    ValidIssuer = setting.Issuer,// Issuer，这两项和前面签发jwt的设置一致
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(setting.SecretKey))// 拿到SecurityKey
                };
                config.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        // 如果过期，则把<是否过期>添加到返回头信息中
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            });
            #endregion



            #endregion

            // 注入权限处理器
            services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

            services.AddMvc();

            #region Swagger UI
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API171", Version = "v1" });
                //获取应用程序所在目录（绝对，不受工作目录影响，建议采用此方法获取路径）
                var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);
                var xmlPath = Path.Combine(basePath, "ApiServer.xml");
                c.IncludeXmlComments(xmlPath);
            });
            #endregion

            services.AddMvc(options =>
            {
                // 注册全局过滤器
                options.Filters.Add<GlobalExceptionFilter>();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // 配置HTTP请求管道
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseRouting();

            // 添加jwt验证
            app.UseAuthentication();

            
            // 添加请求日志中间件
            app.UseSerilogRequestLogging();

            #region Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                //要在应用的根(http://localhost:<port>/) 处提供 Swagger UI，请将 RoutePrefix 属性设置为空字符串
                c.RoutePrefix = string.Empty;
                //swagger集成auth验证
            });
            #endregion

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
