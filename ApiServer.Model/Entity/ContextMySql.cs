using Microsoft.EntityFrameworkCore;

namespace ApiServer.Model
{
    public class ContextMySql : DbContext
    {
        public ContextMySql()
        {
        }

        public ContextMySql(DbContextOptions<ContextMySql> options)
            : base(options)
        {
        }
    }
}
