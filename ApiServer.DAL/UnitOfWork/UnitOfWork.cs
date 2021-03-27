using ApiServer.Model.Entity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ApiServer.DAL.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ContextMySql myDbContext;

        public UnitOfWork(ContextMySql myDbContext)
        {
            this.myDbContext = myDbContext;
        }

        public DbContext GetDbContext()
        {
            return myDbContext;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await myDbContext.SaveChangesAsync();
        }
    }
}
