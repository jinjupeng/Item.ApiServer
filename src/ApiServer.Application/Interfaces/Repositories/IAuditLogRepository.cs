using ApiServer.Domain.Entities;
using ApiServer.Application.Interfaces.Repositories;

namespace ApiServer.Application.Interfaces.Repositories
{
    /// <summary>
    /// 审计日志仓储接口
    /// </summary>
    public interface IAuditLogRepository : IBaseRepository<AuditLog>
    {
    }
}
