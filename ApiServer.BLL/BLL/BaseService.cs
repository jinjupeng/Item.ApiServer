using ApiServer.Model.Entity;
using Item.ApiServer.BLL.IBLL;
using Item.ApiServer.DAL.IDAL;
using System.Collections.Generic;

namespace Item.ApiServer.BLL.BLL
{
    public class BaseService<T> : IBaseService<T> where T : class
    {
        private readonly IBaseDal<T> _baseDal;

        public BaseService(IBaseDal<T> baseDal)
        {
            _baseDal = baseDal;
        }


    }
}