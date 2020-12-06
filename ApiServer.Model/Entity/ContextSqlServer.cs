using Microsoft.EntityFrameworkCore;

namespace ApiServer.Model.Entity
{
    public class ContextSqlServer : DbContext
    {
        public ContextSqlServer()
        {

        }

        public ContextSqlServer(DbContextOptions<ContextSqlServer> options)
            : base(options)
        {
        }
    }
}
