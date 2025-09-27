namespace ApiServer.Application.DTOs.User
{
    /// <summary>
    /// 用户查询DTO
    /// </summary>
    public class UserQueryDto
    {
        /// <summary>
        /// 页码
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// 页码索引（与Page相同，为了兼容性）
        /// </summary>
        public int PageIndex 
        { 
            get => Page; 
            set => Page = value; 
        }

        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// 关键字（用户名、昵称、邮箱、手机号）
        /// </summary>
        public string? Keyword { get; set; }

        /// <summary>
        /// 组织ID
        /// </summary>
        public long? OrgId { get; set; }

        /// <summary>
        /// 用户状态
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }
    }
}