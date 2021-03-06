﻿using ApiServer.BLL.IBLL;
using ApiServer.JWT;
using ApiServer.Model.Model;
using ApiServer.Model.Model.MsgModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiServer.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    public class JwtAuthController : BaseController
    {
        private readonly IJwtAuthService _jwtAuthService;
        private readonly ISysRoleService _sysRoleService;
        private readonly IHttpContextAccessor _accessor;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jwtAuthService"></param>
        /// <param name="sysRoleService"></param>
        public JwtAuthController(IJwtAuthService jwtAuthService, ISysRoleService sysRoleService,
            IHttpContextAccessor accessor)
        {
            _jwtAuthService = jwtAuthService;
            _sysRoleService = sysRoleService;
            _accessor = accessor;
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
                TokenModelJwt tokenModel = new TokenModelJwt
                {
                    Name = username,
                    Role = _sysRoleService.GetRoleByUserName(username)
                };
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
            var httpContext = _accessor.HttpContext;
            //获取请求头部信息token
            var result = httpContext.Request.Headers.TryGetValue("Authorization", out StringValues oldToken);
            //判断token是否为空
            if (!result || string.IsNullOrEmpty(oldToken.ToString()))
            {
                return Ok(await Task.FromResult(MsgModel.Error(new CustomException(StatusCodes.Status401Unauthorized, "用户登录信息已失效，请重新登录！"))));
            }
            string refreshToken = JwtHelper.RefreshToken(oldToken);
            return Ok(await Task.FromResult(MsgModel.Success(refreshToken)));
        }
    }
}
