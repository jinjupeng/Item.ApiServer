using ApiServer.BLL.BLL;
using ApiServer.BLL.IBLL;
using ApiServer.Model.Enum;
using ApiServer.Model.Model.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ApiServer.Extensions.ServiceExensions
{
    public static class CacheExtension
    {
        /// <summary>
        /// 添加缓存功能
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddCache(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CacheConfig>(configuration.GetSection("Cache"));
            var cacheConfig = new CacheConfig();
            configuration.Bind("Cache", cacheConfig);

            if (cacheConfig.Provider == CacheProvider.MemoryCache)
            {
                #region MemoryCache缓存

                services.AddMemoryCache(options =>
                {
                    // SizeLimit缓存是没有大小的，此值设置缓存的份数
                    // 注意：netcore中的缓存是没有单位的，缓存项和缓存的相对关系
                    options.SizeLimit = 1024;
                    // 缓存满的时候压缩20%的优先级较低的数据
                    options.CompactionPercentage = 0.2;
                    // 两秒钟查找一次过期项
                    options.ExpirationScanFrequency = TimeSpan.FromSeconds(2);
                });
                // MemoryCache缓存注入
                services.AddTransient<ICacheService, MemoryCacheService>();

                #endregion
            }
            else if (cacheConfig.Provider == CacheProvider.Redis)
            {
                #region Redis缓存

                services.AddDistributedRedisCache(options =>
                {
                    options.InstanceName = cacheConfig.Redis.Prefix;
                    options.Configuration = cacheConfig.Redis.ConnectionString;
                    options.ConfigurationOptions.DefaultDatabase = cacheConfig.Redis.DefaultDb;
                });
                // Redis缓存注入
                services.AddSingleton<ICacheService, RedisCacheService>();

                #endregion
            }
            return services;
        }
    }
}
