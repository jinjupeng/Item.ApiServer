using ApiServer.BLL.IBLL;
using ApiServer.Model.Entity;
using Item.ApiServer.BLL.IBLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApiServer.BLL.BLL
{
    public class SysDictService : ISysDictService
    {
        private readonly IBaseService<Sys_Dict> _baseSysDictService;

        public SysDictService(IBaseService<Sys_Dict> baseSysDictService)
        {
            _baseSysDictService = baseSysDictService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupName">分组名称</param>
        /// <param name="groupCode">分组编码</param>
        /// <returns></returns>
        public List<Sys_Dict> Query(string groupName, string groupCode)
        {
            return _baseSysDictService.GetModels(a => a.group_name == groupName && a.group_code == groupCode).ToList();

        }

        /// <summary>
        /// 更新数据字典项
        /// </summary>
        /// <param name="sys_Dict"></param>
        public void Update(Sys_Dict sys_Dict)
        {
            _baseSysDictService.UpdateRange(sys_Dict);
        }

        /// <summary>
        /// 新增数据字典项
        /// </summary>
        /// <param name="sys_Dict"></param>
        public void Add(Sys_Dict sys_Dict)
        {
            _baseSysDictService.AddRange(sys_Dict);
        }

        /// <summary>
        /// 根据id删除数据字典项
        /// </summary>
        /// <param name="id"></param>
        public void Delete(long id)
        {
            _baseSysDictService.DeleteRange(_baseSysDictService.GetModels(a => a.id == id));
        }
    }
}
