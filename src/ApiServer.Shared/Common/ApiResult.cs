namespace ApiServer.Shared.Common
{
    /// <summary>
    /// API统一返回结果
    /// </summary>
    public class ApiResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 状态码
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// 时间戳
        /// </summary>
        public long Timestamp { get; set; } = DateTimeOffset.Now.ToUnixTimeSeconds();

        /// <summary>
        /// 成功结果
        /// </summary>
        public static ApiResult SuccessResult(string message = "操作成功")
        {
            return new ApiResult
            {
                Success = true,
                Code = 200,
                Message = message
            };
        }

        /// <summary>
        /// 失败结果
        /// </summary>
        public static ApiResult FailResult(string message = "操作失败", int code = 400)
        {
            return new ApiResult
            {
                Success = false,
                Code = code,
                Message = message
            };
        }
    }

    /// <summary>
    /// 带数据的API统一返回结果
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public class ApiResult<T> : ApiResult
    {
        /// <summary>
        /// 数据
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// 成功结果
        /// </summary>
        public static new ApiResult<T> SuccessResult(T data, string message = "操作成功")
        {
            return new ApiResult<T>
            {
                Success = true,
                Code = 200,
                Message = message,
                Data = data
            };
        }

        /// <summary>
        /// 失败结果
        /// </summary>
        public static new ApiResult<T> FailResult(string message = "操作失败", int code = 400)
        {
            return new ApiResult<T>
            {
                Success = false,
                Code = code,
                Message = message,
                Data = default(T)
            };
        }
    }
}