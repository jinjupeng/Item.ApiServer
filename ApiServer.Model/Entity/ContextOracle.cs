using Microsoft.EntityFrameworkCore;

namespace ApiServer.Model.Entity
{
    public class ContextOracle : DbContext
    {
        public ContextOracle()
        {
        }

        public ContextOracle(DbContextOptions<ContextOracle> options)
            : base(options)
        {
        }
    }
}
