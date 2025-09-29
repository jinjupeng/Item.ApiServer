using ApiServer.Application.Interfaces.Repositories;
using ApiServer.Domain.Entities;
using ApiServer.Infrastructure.Data;
using ApiServer.Infrastructure.Repositories;

namespace ApiServer.Infrastructure.Repositories
{
    /// <summary>
    /// �����־�ִ�
    /// </summary>
    public class AuditLogRepository : BaseRepository<AuditLog>, IAuditLogRepository
    {
        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="context"></param>
        public AuditLogRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
