﻿using ApiServer.Models.Entity;
using ApiServer.Models.Model.MsgModel;
using ApiServer.Models.Model.ViewModel;
using Common.Utility.Utils;
using LinqKit;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ApiServer.BLL.BLL
{
    public interface ISysDictService
    {
        MsgModel All();
        MsgModel Query(string groupName, string groupCode);

        MsgModel Update(sys_dict sys_Dict);
        MsgModel Add(sys_dict sys_Dict);
        MsgModel Delete(long id);
    }
    public class SysDictService : ISysDictService
    {
        private readonly IBaseService<sys_dict> _baseSysDictService;

        public SysDictService(IBaseService<sys_dict> baseSysDictService)
        {
            _baseSysDictService = baseSysDictService;
        }

        /// <summary>
        /// 查询所有
        /// </summary>
        /// <returns></returns>
        public MsgModel All()
        {
            List<sys_dict> list = _baseSysDictService.GetModels(null).ToList();
            var data = list.BuildAdapter().AdaptToType<List<SysDict>>();
            return MsgModel.Success(data, "查询成功！");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupName">分组名称</param>
        /// <param name="groupCode">分组编码</param>
        /// <returns></returns>
        public MsgModel Query(string groupName, string groupCode)
        {
            Expression<Func<sys_dict, bool>> where = PredicateBuilder.True<sys_dict>();
            if (!string.IsNullOrWhiteSpace(groupName))
            {
                where = where.And(a => a.group_name.Contains(groupName));
            }
            if (!string.IsNullOrWhiteSpace(groupCode))
            {
                where = where.And(a => a.group_name.Contains(groupCode));
            }
            var sysDictList = _baseSysDictService.GetModels(where).ToList();

            return MsgModel.Success(sysDictList.BuildAdapter().AdaptToType<List<SysDict>>(), "查询成功！");
        }

        /// <summary>
        /// 更新数据字典项
        /// </summary>
        /// <param name="sys_Dict"></param>
        public MsgModel Update(sys_dict sys_Dict)
        {
            var result = _baseSysDictService.UpdateRange(sys_Dict);
            return result ? MsgModel.Success("更新数据字典项成功！") : MsgModel.Fail("更新数据字典项失败！");
        }

        /// <summary>
        /// 新增数据字典项
        /// </summary>
        /// <param name="sys_Dict"></param>
        public MsgModel Add(sys_dict sys_Dict)
        {
            sys_Dict.id = new Snowflake().GetId();
            var result = _baseSysDictService.AddRange(sys_Dict);
            return result ? MsgModel.Success("新增数据字典项成功！") : MsgModel.Fail("新增数据字典项失败！");
        }

        /// <summary>
        /// 根据id删除数据字典项
        /// </summary>
        /// <param name="id"></param>
        public MsgModel Delete(long id)
        {
            var result = _baseSysDictService.DeleteRange(_baseSysDictService.GetModels(a => a.id == id));
            return result ? MsgModel.Success("删除数据字典项成功！") : MsgModel.Fail("删除数据字典项失败！");
        }
    }
}
