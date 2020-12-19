﻿using ApiServer.Model.Model;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ApiServer.JWT
{
    public class JwtHelper
    {
        private static JwtSettings settings;
        public static JwtSettings Settings { set { settings = value; } }

        /// <summary>
        /// 秘钥，可以从配置文件中获取
        /// </summary>
        public static string SecurityKey = Common.ConfigTool.Configuration["Jwt:SecurityKey"];

        /// <summary>
        /// 颁发JWT字符串
        /// </summary>
        /// <param name="tokenModel"></param>
        /// <returns></returns>
        public static string IssueJwt(TokenModelJwt tokenModel)
        {
            // 这里就是声明我们的claim
            var claims = new Claim[] {
                    #region token添加自定义参数
                    new Claim(ClaimTypes.Name, tokenModel.Name),
                    new Claim(ClaimTypes.Role, tokenModel.Role),
                    // new Claim(ClaimTypes.Sid,tokenModel.ToString()),
                    #endregion
                    new Claim(JwtRegisteredClaimNames.Jti, tokenModel.Sid.ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, $"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}"),
                    new Claim(JwtRegisteredClaimNames.Nbf,$"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}") ,
                    // 这个就是过期时间，目前是过期60秒，可自定义，注意JWT有自己的缓冲过期时间
                    new Claim (JwtRegisteredClaimNames.Exp,$"{new DateTimeOffset(DateTime.Now.AddSeconds(60)).ToUnixTimeSeconds()}"),
                    new Claim(JwtRegisteredClaimNames.Iss,settings.Issuer),
                    new Claim(JwtRegisteredClaimNames.Aud,settings.Audience),
                };

            // 密钥(SymmetricSecurityKey 对安全性的要求，密钥的长度太短会报出异常)
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: settings.Issuer,
                // audience: settings.Audience,
                claims: claims,
                // expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);
            var Token = new JwtSecurityTokenHandler().WriteToken(token);
            return Token;
        }

        /// <summary>
        /// 解析token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static TokenModelJwt SerializeJwt(string token)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken = jwtHandler.ReadJwtToken(token);
            object role;
            try
            {
                jwtToken.Payload.TryGetValue(ClaimTypes.Role, out role);
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            var tm = new TokenModelJwt
            {
                Sid = Convert.ToInt64(jwtToken.Id),
                Role = role != null ? Convert.ToString(role) : "",
            };
            return tm;
        }

        /// <summary>
        /// 验证身份 验证签名的有效性,
        /// </summary>
        /// <param name="encodeJwt"></param>
        /// 例如：payLoad["aud"]?.ToString() == "roberAuddience";
        /// 例如：验证是否过期 等
        /// <returns></returns>
        public static bool Validate(string encodeJwt)
        {
            var success = true;
            //encodeJwt = encodeJwt.ToString().Substring("Bearer ".Length).Trim();
            var jwtArr = encodeJwt.Split('.');
            //var header = JsonConvert.DeserializeObject<Dictionary<string, object>>(Base64UrlEncoder.Decode(jwtArr[0]));
            var payLoad = JsonConvert.DeserializeObject<Dictionary<string, object>>(Base64UrlEncoder.Decode(jwtArr[1]));

            var hs256 = new HMACSHA256(Encoding.ASCII.GetBytes(SecurityKey));

            var encodedSignature = Base64UrlEncoder.Encode(hs256.ComputeHash(Encoding.UTF8.GetBytes(string.Concat(jwtArr[0], ".", jwtArr[1]))));

            //首先验证签名是否正确（必须的)
            success = success && (string.Equals(jwtArr[2], encodedSignature));
            if (!success)
            {
                return success;
            }

            //其次验证是否在有效期内（也应该必须）
            var now = ToUnixEpochDate(DateTime.UtcNow);
            success = success && (now <= long.Parse(payLoad["exp"].ToString()) && now >= long.Parse(payLoad["nbf"].ToString()));
            return success;
        }

        /// <summary>
        /// 获取jwt中的payload
        /// </summary>
        /// <param name="encodeJwt"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetPayLoad(string encodeJwt)
        {
            var jwtArr = encodeJwt.Split(',');
            var payLoad = JsonConvert.DeserializeObject<Dictionary<string, object>>(Base64UrlEncoder.Decode(jwtArr[1]));
            return payLoad;
        }

        /// <summary>
        /// datetime转时间戳
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static long ToUnixEpochDate(DateTime date) =>
            (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);

    }
}
