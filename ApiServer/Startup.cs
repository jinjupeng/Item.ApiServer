using ApiServer.Exception;
using ApiServer.JWT;
using ApiServer.Middleware;
using Autofac;
using Item.ApiServer.BLL.BLLModule;
using Item.ApiServer.DAL.DALModule;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Serilog;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ApiServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public IContainer Container { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // 使用DI将服务注入到容器中
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddControllers().AddNewtonsoftJson(
            //    options =>
            //    {
            //        //序列化时忽略循环
            //        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            //        //不使用驼峰命名
            //        options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            //        //Enum转换为字符串
            //        options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            //        //序列化时是否忽略空值
            //        options.SerializerSettings.NullValueHandling = NullValueHandling.Include;
            //        //序列化时的时间格式
            //        options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            //    });

            //services.Configure<FormOptions>(options =>
            //{
            //    options.MultipartBodyLengthLimit = int.MaxValue;
            //    options.ValueLengthLimit = int.MaxValue;
            //    options.MemoryBufferThreshold = int.MaxValue;
            //});

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            #region JwtSetting类注入
            services.Configure<JwtSettings>(Configuration.GetSection("JwtSettings"));
            JwtSettings setting = new JwtSettings();
            Configuration.Bind("JwtSettings", setting);
            JwtHelper.Settings = setting;
            #endregion

            #region 基于策略模式的授权
            // jwt服务注入
            services.AddAuthorization(options =>
            {
                // 增加定义策略
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
                    //OnAuthenticationFailed = context =>
                    //{
                    //    // 如果过期，则把<是否过期>添加到返回头信息中
                    //    if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                    //    {
                    //        context.Response.Headers.Add("Token-Expired", "true");
                    //    }
                    //    return Task.CompletedTask;
                    //}
                    //此处为权限验证失败后触发的事件
                    OnChallenge = context =>
                    {
                        //此处代码为终止.Net Core默认的返回类型和数据结果，这个很重要哦，必须
                        context.HandleResponse();
                        if (!context.Response.HasStarted)
                        {
                            //自定义自己想要返回的数据结果，我这里要返回的是Json对象，通过引用Newtonsoft.Json库进行转换
                            var payload = JsonConvert.SerializeObject(new { Code = 0, Message = "很抱歉，您无权访问该接口!" });
                            //自定义返回的数据类型
                            context.Response.ContentType = "application/json";
                            //自定义返回状态码，默认为401 我这里改成 200
                            context.Response.StatusCode = StatusCodes.Status200OK;
                            //context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            //输出Json数据结果
                            context.Response.WriteAsync(payload);
                        }

                        return Task.FromResult(0);
                    }
                };
            });
            #endregion



            #endregion

            #region Cors 跨域
            services.AddCors(options =>
            {
                // 浏览器会发起2次请求,使用OPTIONS发起预检请求，第二次才是api请求
                options.AddPolicy("cors", policy =>
                {
                    policy
                    .SetIsOriginAllowed(origin => true)
                    .SetPreflightMaxAge(new TimeSpan(0, 10, 0))
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials(); //指定处理cookie
                });
            });
            #endregion

            // 注入自定义策略
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

            app.UseCors("cors");

            // app.UseMiddleware<RefererMiddleware>(); // 判断Referer请求来源是否合法
            app.UseMiddleware<ExceptionMiddleware>(); // 全局异常过滤
            app.UseRouting();

            app.UseAuthorization();
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

        /// <summary>
        /// 自动注册
        /// </summary>
        /// <param name="builder"></param>
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new BllModule());
            builder.RegisterModule(new DalModule());

        }
    }
}
