namespace ApiServer.Model.Model.MsgModel
{
    public class MsgModel
    {
        public bool isok { get; set; }  //请求是否处理成功

        public int code { get; set; } //请求响应状态码（200、400、500）

        public string message { get; set; } //请求结果描述信息

        public object data { get; set; }//请求结果数据（通常用于查询操作）
    }
}
