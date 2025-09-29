using ApiServer.Domain.Entities;
using ApiServer.Application.Interfaces.Repositories;

namespace ApiServer.Application.Interfaces.Repositories
{
    /// <summary>
    /// �����־�ִ��ӿ�
    /// </summary>
    public interface IAuditLogRepository : IBaseRepository<AuditLog>
    {
    }
}
