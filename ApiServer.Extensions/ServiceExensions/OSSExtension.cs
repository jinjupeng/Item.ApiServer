using ApiServer.Model.Model.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ApiServer.Extensions.ServiceExensions
{
    public static class OSSExtension
    {
        /// <summary>
        /// 添加OSS功能
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddOSS(this IServiceCollection services, IConfiguration configuration)
        {
            var config = new OSSConfig();
            var section = configuration.GetSection("OSS");
            if (section != null)
            {
                section.Bind(config);
            }
            services.AddSingleton(config);
            services.Configure<OSSConfig>(configuration.GetSection("OSS"));

            return services;
        }
    }
}
