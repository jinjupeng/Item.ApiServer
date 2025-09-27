namespace ApiServer.Application.DTOs.Common
{
    /// <summary>
    /// 基础DTO
    /// </summary>
    public abstract class BaseDto
    {
        /// <summary>
        /// ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }

    /// <summary>
    /// 可审计DTO基类
    /// </summary>
    public abstract class AuditableDto : BaseDto
    {
        /// <summary>
        /// 创建人ID
        /// </summary>
        public long? CreatedBy { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime? LastModifiedTime { get; set; }

        /// <summary>
        /// 最后修改人ID
        /// </summary>
        public long? LastModifiedBy { get; set; }
    }

    /// <summary>
    /// 分页查询基类
    /// </summary>
    public class PagedQueryDto
    {
        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex { get; set; } = 1;

        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { get; set; } = 20;

        /// <summary>
        /// 排序字段
        /// </summary>
        public string? OrderBy { get; set; }

        /// <summary>
        /// 是否降序
        /// </summary>
        public bool IsDescending { get; set; } = false;
    }

    /// <summary>
    /// 分页结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagedResultDto<T>
    {
        /// <summary>
        /// 数据列表
        /// </summary>
        public IList<T> Items { get; set; } = new List<T>();

        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        /// <summary>
        /// 是否有上一页
        /// </summary>
        public bool HasPreviousPage => PageIndex > 1;

        /// <summary>
        /// 是否有下一页
        /// </summary>
        public bool HasNextPage => PageIndex < TotalPages;
    }
}
