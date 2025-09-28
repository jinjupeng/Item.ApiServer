namespace ApiServer.Domain.Enums
{
    /// <summary>
    /// 用户状态枚举
    /// </summary>
    public enum UserStatus
    {
        /// <summary>
        /// 禁用
        /// </summary>
        Disabled = 0,
        
        /// <summary>
        /// 启用
        /// </summary>
        Enabled = 1,
        
        /// <summary>
        /// 活跃
        /// </summary>
        Active = 1,
        
        /// <summary>
        /// 非活跃
        /// </summary>
        Inactive = 2,
        
        /// <summary>
        /// 锁定
        /// </summary>
        Locked = 3
    }

    /// <summary>
    /// 菜单类型枚举
    /// </summary>
    public enum PermissionType
    {
        /// <summary>
        /// 目录
        /// </summary>
        Directory = 0,
        
        /// <summary>
        /// 菜单
        /// </summary>
        Menu = 1,
        
        /// <summary>
        /// 按钮/Api
        /// </summary>
        Button = 2
    }

    /// <summary>
    /// 性别枚举
    /// </summary>
    public enum Gender
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unknown = 0,
        
        /// <summary>
        /// 男
        /// </summary>
        Male = 1,
        
        /// <summary>
        /// 女
        /// </summary>
        Female = 2
    }

    /// <summary>
    /// 操作类型枚举
    /// </summary>
    public enum OperationType
    {
        /// <summary>
        /// 查询
        /// </summary>
        Query = 0,
        
        /// <summary>
        /// 新增
        /// </summary>
        Create = 1,
        
        /// <summary>
        /// 修改
        /// </summary>
        Update = 2,
        
        /// <summary>
        /// 删除
        /// </summary>
        Delete = 3
    }

    /// <summary>
    /// 缓存提供者枚举
    /// </summary>
    public enum CacheProvider
    {
        /// <summary>
        /// 内存缓存
        /// </summary>
        Memory = 0,

        /// <summary>
        /// Redis缓存
        /// </summary>
        Redis = 1
    }

    /// <summary>
    /// HTTP状态码枚举
    /// </summary>
    public enum HttpStatusCode
    {
        /// <summary>
        /// 成功
        /// </summary>
        Success = 200,

        /// <summary>
        /// 未授权
        /// </summary>
        Unauthorized = 401,

        /// <summary>
        /// 禁止访问
        /// </summary>
        Forbidden = 403,

        /// <summary>
        /// 未找到
        /// </summary>
        NotFound = 404,

        /// <summary>
        /// 服务器内部错误
        /// </summary>
        InternalServerError = 500
    }

    /// <summary>
    /// 文件大小单位枚举
    /// </summary>
    public enum FileSizeUnit
    {
        /// <summary>
        /// 字节
        /// </summary>
        Byte = 0,

        /// <summary>
        /// KB
        /// </summary>
        KB = 1,

        /// <summary>
        /// MB
        /// </summary>
        MB = 2,

        /// <summary>
        /// GB
        /// </summary>
        GB = 3
    }

    /// <summary>
    /// 文件访问模式枚举
    /// </summary>
    public enum FileAccessMode
    {
        /// <summary>
        /// 只读
        /// </summary>
        ReadOnly = 0,

        /// <summary>
        /// 读写
        /// </summary>
        ReadWrite = 1
    }
}
