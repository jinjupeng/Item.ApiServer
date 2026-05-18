namespace ApiServer.Shared.Common
{
    /// <summary>
    /// API统一返回结果
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public class ApiResult<T>
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
        /// 数据
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public long Timestamp { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        /// <summary>
        /// 创建成功结果
        /// </summary>
        public static ApiResult<T> Succeed(T? data = default, string message = "操作成功")
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
        /// 创建失败结果
        /// </summary>
        public static ApiResult<T> Failed(string message = "操作失败", int code = 400)
        {
            return new ApiResult<T>
            {
                Success = false,
                Code = code,
                Message = message
            };
        }
    }

    /// <summary>
    /// API统一返回结果（无泛型）
    /// </summary>
    public class ApiResult : ApiResult<object>
    {
        /// <summary>
        /// 创建成功结果
        /// </summary>
        public static new ApiResult Succeed(object? data = null, string message = "操作成功")
        {
            return new ApiResult
            {
                Success = true,
                Code = 200,
                Message = message,
                Data = data
            };
        }

        /// <summary>
        /// 创建失败结果
        /// </summary>
        public static new ApiResult Failed(string message = "操作失败", int code = 400)
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
    /// API结果工厂
    /// </summary>
    public static class ApiResultFactory
    {
        public static ApiResult BadRequest(string message) => ApiResult.Failed(message, 400);

        public static ApiResult<T> BadRequest<T>(string message) => ApiResult<T>.Failed(message, 400);

        public static ApiResult Unauthorized(string message) => ApiResult.Failed(message, 401);

        public static ApiResult<T> Unauthorized<T>(string message) => ApiResult<T>.Failed(message, 401);

        public static ApiResult Forbidden(string message) => ApiResult.Failed(message, 403);

        public static ApiResult<T> Forbidden<T>(string message) => ApiResult<T>.Failed(message, 403);

        public static ApiResult NotFound(string message) => ApiResult.Failed(message, 404);

        public static ApiResult<T> NotFound<T>(string message) => ApiResult<T>.Failed(message, 404);

        public static ApiResult Conflict(string message) => ApiResult.Failed(message, 409);

        public static ApiResult<T> Conflict<T>(string message) => ApiResult<T>.Failed(message, 409);
    }
}
