using System.Collections.Generic;

namespace ApiServer.Model.Model
{
    /// <summary>
    /// 通用分页信息类
    /// </summary>
    public class PageModel<T>
    {
        /// <summary>
        /// 当前页标
        /// </summary>
        public int PageIndex { get; set; } = 1;
        /// <summary>
        /// 总页数
        /// </summary>
        public int PageCount { get; set; } = 10;
        /// <summary>
        /// 数据总数
        /// </summary>
        public int total { get; set; } = 0;
        /// <summary>
        /// 每页大小
        /// </summary>
        public int size { set; get; }
        /// <summary>
        /// 返回数据
        /// </summary>
        public List<T> records { get; set; }

    }
}
