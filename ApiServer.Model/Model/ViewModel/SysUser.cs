using System;

namespace ApiServer.Model.Model.ViewModel
{
    public class SysUser
    {
        public long id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string username { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string password { get; set; }

        /// <summary>
        /// 组织id
        /// </summary>
        public long orgId { get; set; }

        /// <summary>
        /// 0无效用户，1是有效用户
        /// </summary>
        public bool? enabled { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string phone { get; set; }

        /// <summary>
        /// email
        /// </summary>
        public string email { get; set; }

        /// <summary>
        /// 用户创建时间
        /// </summary>
        public DateTime create_time { get; set; }
    }
}
