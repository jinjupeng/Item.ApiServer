using CoreJWT;
using CoreJWT.JWT;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;
using System.Text;
using System.Threading.Tasks;

namespace CoreJWT
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        private const string ApiName = "CoreJwt";

        // This method gets called by the runtime. Use this method to add services to the container.
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
                        if(context.Exception.GetType() == typeof(SecurityTokenExpiredException))
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

            #region Swagger UI
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    // {ApiName} 定义成全局变量，方便修改
                    Version = "v1",
                    Title = $"{ApiName} 接口文档",
                    Description = $"{ApiName} HTTP API ",
                    TermsOfService = "None",
                    Contact = new Contact { Name = "corejwt", Email = "im.jp@outlook.com", Url = "https://github.com/jinjupeng/CoreJWT" }
                });
            });
            #endregion
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // 添加jwt验证
            app.UseAuthentication();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            #region Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/v1/swagger.json", $"{ApiName} v1");
                c.RoutePrefix = ""; //路径配置，设置为空，表示直接在根域名（localhost:8001）访问该文件,注意localhost:8001/swagger是访问不到的，去launchSettings.json把launchUrl去掉
            });
            #endregion
            app.UseMvc();
        }
    }
}
