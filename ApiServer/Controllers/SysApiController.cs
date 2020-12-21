using ApiServer.BLL.IBLL;
using ApiServer.Model.Entity;
using ApiServer.Model.Model;
using ApiServer.Model.Model.MsgModel;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiServer.Controllers
{
    [Route("api/[controller]")]
    public class SysApiController : BaseController
    {
        private readonly ISysApiService _sysApiService;

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
            MsgModel msg = new MsgModel
            {
                message = "查询成功！"
            };
            List<SysApiNode> list = _sysApiService.GetApiTreeById(apiNameLike, apiStatus);
            msg.data = list;
            return Ok(await Task.FromResult(msg));
        }


        /// <summary>
        /// 接口管理:新增
        /// </summary>
        /// <param name="sys_Api"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> Add([FromBody] Sys_Api sys_Api)
        {
            MsgModel msg = new MsgModel
            {
                message = "新增接口配置成功！"
            };
            _sysApiService.AddApi(sys_Api);
            return Ok(await Task.FromResult(msg));
        }

        /// <summary>
        /// 接口管理:修改
        /// </summary>
        /// <param name="sys_Api"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> Update([FromBody] Sys_Api sys_Api)
        {
            MsgModel msg = new MsgModel
            {
                message = "修改接口配置成功！"
            };
            _sysApiService.UpdateApi(sys_Api);
            return Ok(await Task.FromResult(msg));
        }

        /// <summary>
        /// 接口管理:删除
        /// </summary>
        /// <param name="sys_Api"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("delete")]
        public async Task<IActionResult> Delete([FromBody] Sys_Api sys_Api)
        {
            MsgModel msg = new MsgModel
            {
                message = "删除接口配置成功！"
            };
            _sysApiService.DeleteApi(sys_Api);
            return Ok(await Task.FromResult(msg));
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
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.Add("tree", _sysApiService.GetApiTreeById("", default));
            dict.Add("expandedKeys", _sysApiService.GetExpandedKeys());
            dict.Add("checkedKeys", _sysApiService.GetCheckedKeys(roleId));

            return Ok(await Task.FromResult(dict));

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
            MsgModel msg = new MsgModel
            {
                message = "保存接口权限成功！"
            };
            _sysApiService.SaveCheckedKeys(roleCheckedIds.RoleId, roleCheckedIds.CheckedIds);

            return Ok(await Task.FromResult(msg));

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
            MsgModel msg = new MsgModel
            {
                message = "接口禁用状态更新成功！"
            };
            _sysApiService.UpdateStatus(apiId, status);

            return Ok(await Task.FromResult(msg));

        }
    }
}
