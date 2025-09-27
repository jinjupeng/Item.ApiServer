using System.ComponentModel.DataAnnotations;

namespace ApiServer.Domain.Common
{
    /// <summary>
    /// 基础实体类
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// 可审计实体基类
    /// </summary>
    public abstract class AuditableEntity : BaseEntity
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
    /// 软删除实体基类
    /// </summary>
    public abstract class SoftDeleteEntity : AuditableEntity
    {
        /// <summary>
        /// 是否已删除
        /// </summary>
        public bool IsDeleted { get; set; } = false;

        /// <summary>
        /// 删除时间
        /// </summary>
        public DateTime? DeletedTime { get; set; }

        /// <summary>
        /// 删除人ID
        /// </summary>
        public long? DeletedBy { get; set; }
    }
}