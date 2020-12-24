namespace ApiServer.Model.Model.MsgModel
{
    public class MsgModel
    {
        /// <summary>
        /// 请求是否处理成功
        /// </summary>
        public bool isok { get; set; } = true;

        /// <summary>
        /// 请求响应状态码（200、400、500）
        /// </summary>
        public int code { get; set; } = 200;

        /// <summary>
        /// 请求结果描述信息
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// 请求结果数据（通常用于查询操作）
        /// </summary>
        public object data { get; set; }
    }
}
