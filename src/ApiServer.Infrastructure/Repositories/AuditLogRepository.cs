using ApiServer.Application.Interfaces.Repositories;
using ApiServer.Domain.Entities;
using ApiServer.Infrastructure.Data;
using ApiServer.Infrastructure.Repositories;

namespace ApiServer.Infrastructure.Repositories
{
    /// <summary>
    /// 审计日志仓储
    /// </summary>
    public class AuditLogRepository : BaseRepository<AuditLog>, IAuditLogRepository
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context"></param>
        public AuditLogRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
