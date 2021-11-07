using ApiServer.Models.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiServer.Models.Model.Config
{
    /// <summary>
    /// OSSConfig
    /// </summary>
    public class OSSConfig
    {
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// OSS提供器
        /// </summary>
        public OSSProvider Provider { get; set; } = OSSProvider.Local;

        /// <summary>
        /// 阿里云配置
        /// </summary>
        public AliyunConfig Aliyun { get; set; } = new AliyunConfig();
    }
}
