using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ApiServer.DAL.UnitOfWork
{
    public interface IUnitOfWork
    {
        DbContext GetDbContext();

        Task<int> SaveChangesAsync();
    }
}
