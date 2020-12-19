﻿using ApiServer.JWT;
using ApiServer.Model.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ApiServer.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    public class AuthController : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost]
        [Route("api/auth")]
        public IActionResult Get(string userName, string pwd)
        {
            TokenModelJwt tokenModel = new TokenModelJwt();

            if (CheckAccount(userName, pwd, out string role))
            {
                tokenModel.Name = userName;
                tokenModel.Role = role;
                return Ok(new
                {
                    token = JwtHelper.IssueJwt(tokenModel)
                });
            }
            else
            {
                return BadRequest(new { message = "username or password is incorrect." });
            }
        }

        /// <summary>
        /// 模拟登陆校验，因为是模拟，所以逻辑很‘模拟’
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="pwd"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        private bool CheckAccount(string userName, string pwd, out string role)
        {
            role = "user";

            if (string.IsNullOrEmpty(userName))
                return false;

            if (userName.Equals("admin"))
                role = "admin";

            return true;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("api/nopermission")]
        public IActionResult NoPermission()
        {
            return Forbid();
        }


        [HttpGet]
        [Route("api/value3")]
        [Authorize("Permission")]
        public ActionResult<IEnumerable<string>> Get3()
        {
            // 这是获取自定义参数的方法
            var auth = HttpContext.AuthenticateAsync().Result.Principal.Claims;
            var userName = auth.FirstOrDefault(t => t.Type.Equals(ClaimTypes.Name))?.Value;
            var role = auth.FirstOrDefault(t => t.Type.Equals(ClaimTypes.Role))?.Value;
            return new string[] { "这个接口有管理员权限才可以访问", $"userName={userName}", $"Role={role}" };
        }
    }
}
