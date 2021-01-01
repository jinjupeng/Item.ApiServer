using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using System;
using System.Text;

namespace ApiServer.Cache.MemoryCache
{
    /// <summary>
    /// Redis缓存接口实现
    /// </summary>
    public class RedisCacheService : ICacheService
    {
        /// <summary>
        /// 
        /// </summary>
        protected RedisCache RedisCache = null;

        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="options"></param>
        public RedisCacheService(RedisCacheOptions options)
        {
            RedisCache = new RedisCache(options);
        }


        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expirationTime"></param>
        /// <returns></returns>
        public bool Add(string key, object value, int expirationTime = 20)
        {
            if (!string.IsNullOrEmpty(key))
            {
                RedisCache.Set(key, Encoding.UTF8.GetBytes(value.ToString()), new DistributedCacheEntryOptions()
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(expirationTime)
                });
            }
            return true;
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }

            if (Exists(key))
            {
                RedisCache.Remove(key);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValue(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }
            if (Exists(key))
            {
                return RedisCache.GetString(key);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Exists(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }
            return !string.IsNullOrEmpty(RedisCache.GetString(key));
        }
    }
}
