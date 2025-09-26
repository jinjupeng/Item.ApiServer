using ApiServer.Models;
using ApiServer.Models.Entity;
using ApiServer.Models.Model.MsgModel;
using ApiServer.Models.Model.ViewModel;
using Common.Utility.Helper;
using Common.Utility.JWT;
using Common.Utility.Utils;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace ApiServer.BLL.BLL
{
    public interface IJwtAuthService
    {
        MsgModel Login(string username, string password);

        MsgModel SignUp(SysUser user);
    }

    public class JwtAuthService : IJwtAuthService
    {
        private readonly IBaseService<sys_user> _baseService;
        private readonly JwtHelper _jwtHelper;

        public JwtAuthService(IBaseService<sys_user> baseService, JwtHelper jwtHelper)
        {
            _baseService = baseService;
            _jwtHelper = jwtHelper;
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public MsgModel Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return MsgModel.Fail("用户名或密码为空！");
            }
            // 加密登陆密码
            string encodePassword = PasswordEncoder.Encode(password);

            sys_user sys_user = _baseService.GetModels(a => a.username == username && a.password == encodePassword).SingleOrDefault();
            if (sys_user == null)
            {
                return MsgModel.Fail("用户名或密码不正确！");
            }
            else if (sys_user.enabled == false)
            {
                return MsgModel.Fail("账户已被禁用！");
            }

            // 将一些个人数据写入token中
            var customClaims = new List<Claim>
            {
                new Claim(ClaimAttributes.UserId, Convert.ToString(sys_user.id)),
                new Claim(ClaimAttributes.UserName, username )
            };

            var data = _jwtHelper.IssueJwt(customClaims);
            return MsgModel.Success((object)data);
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public MsgModel SignUp(SysUser user)
        {
            var dict = new Dictionary<string, object>();
            var stringRandom = StringHelper.GenerateRandom(10);
            user.username = stringRandom;
            //user.nickname = stringRandom;
            if (user.phone != null)
            {
                var queryUser = _baseService.GetModels(a => a.phone == user.phone).SingleOrDefault();
                if (queryUser == null)
                {
                    var sysUser = new sys_user();
                    sysUser = user.BuildAdapter().AdaptToType<sys_user>();
                    _baseService.AddRange(sysUser);

                    var customClaims = new List<Claim>
                    {
                        new Claim(ClaimAttributes.UserId, Convert.ToString(queryUser.id)),
                        new Claim(ClaimAttributes.UserName, queryUser.username )
                    };

                    var token = _jwtHelper.IssueJwt(customClaims);
                    dict.Add("token", token);
                    return MsgModel.Success(dict);
                }
                else
                {
                    var userDto = new SysUser();
                    userDto = queryUser.BuildAdapter().AdaptToType<SysUser>();
                    // 用户存在直接登录
                    return Login(userDto.username, userDto.password);
                }
            }
            else
            {
                return MsgModel.Fail("参数格式错误！");
            }
        }
    }
}
