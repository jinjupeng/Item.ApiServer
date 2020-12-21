using ApiServer.BLL.IBLL;
using ApiServer.JWT;
using ApiServer.Model.Model;
using ApiServer.Model.Model.MsgModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiServer.Controllers
{
    [Route("api/[controller]")]
    public class JwtAuthController : BaseController
    {
        private readonly IJwtAuthService _jwtAuthService;

        public JwtAuthController(IJwtAuthService jwtAuthService)
        {
            _jwtAuthService = jwtAuthService;
        }

        /// <summary>
        /// 使用用户名密码来换取jwt令牌
        /// </summary>
        /// <param name="pairs"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("authentication")]
        public async Task<IActionResult> Login([FromBody] Dictionary<string, string> pairs)
        {
            var username = pairs["username"];
            var password = pairs["password"];
            MsgModel msg = _jwtAuthService.Login(username, password);
            if (msg.isok)
            {
                TokenModelJwt tokenModel = new TokenModelJwt();
                tokenModel.Name = username;
                tokenModel.Role = "admin";
                msg.data = JwtHelper.IssueJwt(tokenModel);
            }
            return Ok(await Task.FromResult(msg));
        }

        /// <summary>
        /// 刷新token
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("refreshtoken")]
        public async Task<IActionResult> RefreshToken()
        {
            TokenModelJwt tokenModel = new TokenModelJwt();
            tokenModel.Name = "admin";
            tokenModel.Role = "admin";
            MsgModel msg = new MsgModel
            {
                isok = true,
                data = JwtHelper.IssueJwt(tokenModel)
            };
            return Ok(await Task.FromResult(msg));
        }
    }
}
