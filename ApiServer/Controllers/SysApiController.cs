﻿using ApiServer.BLL.BLL;
using ApiServer.Models.Entity;
using ApiServer.Models.Model.MsgModel;
using ApiServer.Models.Model.Nodes;
using ApiServer.Models.Model.ViewModel;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiServer.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    //[Authorize("Permission")]
    public class SysApiController : BaseController
    {
        private readonly ISysApiService _sysApiService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sysApiService"></param>
        public SysApiController(ISysApiService sysApiService)
        {
            _sysApiService = sysApiService;
        }

        /// <summary>
        /// 接口管理:查询
        /// </summary>
        /// <param name="apiNameLike"></param>
        /// <param name="apiStatus"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("tree")]
        public async Task<IActionResult> Tree([FromForm] string apiNameLike, bool apiStatus)
        {
            return Ok(await Task.FromResult(_sysApiService.GetApiTreeById(apiNameLike, apiStatus)));
        }


        /// <summary>
        /// 接口管理:新增
        /// </summary>
        /// <param name="sysApi"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Add([FromBody] SysApi sysApi)
        {
            var sys_Api = sysApi.BuildAdapter().AdaptToType<sys_api>();
            return Ok(await Task.FromResult(_sysApiService.AddApi(sys_Api)));
        }

        /// <summary>
        /// 接口管理:修改
        /// </summary>
        /// <param name="sysApi"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> Update([FromBody] SysApi sysApi)
        {
            var sys_Api = sysApi.BuildAdapter().AdaptToType<sys_api>();
            return Ok(await Task.FromResult(_sysApiService.UpdateApi(sys_Api)));
        }

        /// <summary>
        /// 接口管理:删除
        /// </summary>
        /// <param name="sysApi"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("delete")]
        public async Task<IActionResult> Delete([FromBody] SysApi sysApi)
        {
            var sys_Api = sysApi.BuildAdapter().AdaptToType<sys_api>();
            return Ok(await Task.FromResult(_sysApiService.DeleteApi(sys_Api)));
        }

        /// <summary>
        /// 角色管理：API树展示（勾选项、展开项）
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("checkedtree")]
        public async Task<IActionResult> CheckedTree([FromForm] long roleId)
        {
            MsgModel msg = new MsgModel
            {
                isok = true,
                message = "获取成功！"
            };
            Dictionary<string, object> dict = new Dictionary<string, object>
            {
                { "tree", _sysApiService.GetApiTreeById("", default).data },
                { "expandedKeys", _sysApiService.GetExpandedKeys() },
                { "checkedKeys", _sysApiService.GetCheckedKeys(roleId) }
            };
            msg.data = dict;
            return Ok(await Task.FromResult(msg));

        }

        /// <summary>
        /// 角色管理：保存API权限勾选结果
        /// </summary>
        /// <param name="roleCheckedIds"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("savekeys")]
        public async Task<IActionResult> SaveKeys([FromBody] RoleCheckedIds roleCheckedIds)
        {
            return Ok(await Task.FromResult(_sysApiService.SaveCheckedKeys(roleCheckedIds.RoleId, roleCheckedIds.CheckedIds)));

        }

        /// <summary>
        /// 接口管理：更新接口禁用状态
        /// </summary>
        /// <param name="apiId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("status/change")]
        public async Task<IActionResult> Update([FromForm] long apiId, bool status)
        {
            return Ok(await Task.FromResult(_sysApiService.UpdateStatus(apiId, status)));

        }
    }
}
