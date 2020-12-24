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

        public MsgModel() { }

        /// <summary>
        /// 请求出现异常时的响应数据封装
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static MsgModel Error(CustomException e)
        {
            MsgModel msg = new MsgModel
            {
                isok = false,
                code = e.Code,
                message = e.Msg
            };
            return msg;
        }

        /// <summary>
        /// 请求出现异常时的响应数据封装
        /// </summary>
        /// <param name="e"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static MsgModel Error(CustomException e, string errorMessage)
        {
            MsgModel msg = new MsgModel
            {
                isok = false,
                code = e.Code,
                message = errorMessage
            };
            return msg;
        }

        /// <summary>
        /// 请求成功的响应，不带查询数据（用于删除、修改、新增接口）
        /// </summary>
        /// <returns></returns>
        public static MsgModel Success()
        {
            MsgModel msg = new MsgModel
            {
                isok = true,
                code = 200,
                message = "请求响应成功!"
            };
            return msg;
        }

        /// <summary>
        /// 请求成功的响应，带有查询数据（用于数据查询接口）
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static MsgModel Success(object obj)
        {
            MsgModel msg = new MsgModel
            {
                isok = true,
                code = 200,
                message = "请求响应成功!",
                data = obj
            };
            return msg;
        }

        /// <summary>
        /// 请求成功的响应，带有查询数据（用于数据查询接口）
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="responseMessage"></param>
        /// <returns></returns>
        public static MsgModel Success(object obj, string responseMessage)
        {
            MsgModel msg = new MsgModel
            {
                isok = true,
                code = 200,
                message = responseMessage,
                data = obj
            };
            return msg;
        }
    }
}
