using ApiServer.BLL.IBLL;
using ApiServer.Common;
using ApiServer.Model.Entity;
using ApiServer.Model.Model.MsgModel;
using ApiServer.Model.Model.ViewModel;
using Mapster;
using System.Collections.Generic;
using System.Linq;

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
        /// 查询所有
        /// </summary>
        /// <returns></returns>
        public MsgModel All()
        {
            MsgModel msg = new MsgModel
            {
                message = "查询成功！",
                isok = true
            };
            TypeAdapterConfig<Sys_Dict, SysDict>.NewConfig().NameMatchingStrategy(NameMatchingStrategy.ToCamelCase);
            List<Sys_Dict> list = _baseSysDictService.GetModels(null).ToList();
            msg.data = list.BuildAdapter().AdaptToType<List<SysDict>>();
            return msg;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupName">分组名称</param>
        /// <param name="groupCode">分组编码</param>
        /// <returns></returns>
        public MsgModel Query(string groupName, string groupCode)
        {
            MsgModel msg = new MsgModel
            {
                message = "查询成功！",
                isok = true
            };
            TypeAdapterConfig<Sys_Dict, SysDict>.NewConfig().NameMatchingStrategy(NameMatchingStrategy.ToCamelCase);
            var sysDictList = _baseSysDictService.GetModels(a => a.group_name.Contains(groupName) && a.group_code.Contains(groupCode)).ToList();

            msg.data = sysDictList.BuildAdapter().AdaptToType<List<SysDict>>();
            return msg;
        }

        /// <summary>
        /// 更新数据字典项
        /// </summary>
        /// <param name="sys_Dict"></param>
        public MsgModel Update(Sys_Dict sys_Dict)
        {
            MsgModel msg = new MsgModel
            {
                isok = true,
                message = "更新数据字典项成功！"
            };
            _baseSysDictService.UpdateRange(sys_Dict);
            return msg;
        }

        /// <summary>
        /// 新增数据字典项
        /// </summary>
        /// <param name="sys_Dict"></param>
        public MsgModel Add(Sys_Dict sys_Dict)
        {
            MsgModel msg = new MsgModel
            {
                isok = true,
                message = "新增数据字典项成功！"
            };
            sys_Dict.id = new Snowflake().GetId();
            _baseSysDictService.AddRange(sys_Dict);
            return msg;
        }

        /// <summary>
        /// 根据id删除数据字典项
        /// </summary>
        /// <param name="id"></param>
        public MsgModel Delete(long id)
        {
            MsgModel msg = new MsgModel
            {
                message = "删除数据字典项成功！"
            };
            _baseSysDictService.DeleteRange(_baseSysDictService.GetModels(a => a.id == id));
            return msg;
        }
    }
}
