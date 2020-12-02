using Item.ApiServer.DAL.IDAL;

namespace Item.ApiServer.DAL.DAL
{
    public class BaseDal<T> : IBaseDal<T> where T : class
    {
        public readonly DbContext DbContext = (new ContextProvider()).GetContext();

        public void AddRange(IEnumerable<T> t)
        {
            DbContext.Set<T>().AddRangeAsync(t);
        }
        public void AddRange(params T[] t)
        {
            DbContext.Set<T>().AddRangeAsync(t);
        }
        public void DeleteRange(IEnumerable<T> t)
        {
            DbContext.Set<T>().RemoveRange(t);
        }
        public void DeleteRange(params T[] t)
        {
            DbContext.Set<T>().RemoveRange(t);
        }
        public void UpdateRange(IEnumerable<T> t)
        {
            DbContext.Set<T>().UpdateRange(t);
        }
        public void UpdateRange(params T[] t)
        {
            DbContext.Set<T>().UpdateRange(t);
        }

        public int CountAll()
        {
            return DbContext.Set<T>().AsNoTracking().Count();
        }


        public IQueryable<T> GetModels(Func<T, bool> whereLambda)
        {
            return whereLambda != null ? DbContext.Set<T>().AsNoTracking().AsEnumerable().Where(whereLambda).AsQueryable().AsNoTracking() : DbContext.Set<T>().AsNoTracking().AsQueryable();
        }
    }
}