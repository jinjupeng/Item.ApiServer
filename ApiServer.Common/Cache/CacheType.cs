using System;
using System.Collections.Generic;
using System.Text;

namespace ApiServer.Common.Cache
{
    /// <summary>
    /// 缓存类型
    /// </summary>
    public enum CacheType
    {
        /// <summary>
        /// 内存缓存
        /// </summary>
        Memory,
        /// <summary>
        /// Redis缓存
        /// </summary>
        Redis
    }
}
