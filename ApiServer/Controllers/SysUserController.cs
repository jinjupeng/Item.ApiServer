using ApiServer.Model.Entity;
using Item.ApiServer.BLL.IBLL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SysUserController : ControllerBase
    {
        private readonly IBaseService<Sys_User> _baseService;

        public SysUserController(IBaseService<Sys_User> baseService)
        {
            _baseService = baseService;
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Login([FromForm] IFormCollection data)
        {
            // return Ok(await Task.FromResult());
            return Ok();
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Exit()
        {
            return Ok();
        }

        /// <summary>
        /// 登录验证
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> LoginCheck()
        {
            return Ok();
        }
    }
}
