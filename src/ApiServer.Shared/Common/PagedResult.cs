namespace ApiServer.Shared.Common
{
    /// <summary>
    /// 分页结果
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public class PagedResult<T>
    {
        /// <summary>
        /// 数据列表
        /// </summary>
        public IEnumerable<T> Items { get; set; } = new List<T>();

        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 每页记录数
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

        /// <summary>
        /// 构造函数
        /// </summary>
        public PagedResult()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="items">数据列表</param>
        /// <param name="total">总记录数</param>
        /// <param name="page">当前页码</param>
        /// <param name="pageSize">每页记录数</param>
        public PagedResult(IEnumerable<T> items, int total, int page, int pageSize)
        {
            Items = items ?? new List<T>();
            TotalCount = total;
            PageIndex = page;
            PageSize = pageSize;
        }

        /// <summary>
        /// 创建空的分页结果
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <returns>空分页结果</returns>
        public static PagedResult<T> Empty(int page = 1, int pageSize = 10)
        {
            return new PagedResult<T>(new List<T>(), 0, page, pageSize);
        }
    }
}