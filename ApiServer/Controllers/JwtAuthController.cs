using ApiServer.Model.Model.MsgModel;
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
    public class JwtAuthController : ControllerBase
    {

        // 使用用户名密码来换取jwt令牌
        [HttpPost]
        [Route("authentication")]
        public async Task<IActionResult> Tree([FromForm]string username, string password)
        {
            MsgModel msg = new MsgModel
            {
                message = "登录成功！"
            };
            if(string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                msg.isok = false;
                msg.message = "用户名或者密码不能为空";
            }
            return Ok(await Task.FromResult(msg));
        }
    }
}
