using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Common.JWT
{
    public class JwtHelper
    {
        private static JwtSettings settings;
        public static JwtSettings Settings { set { settings = value; } }
        /// <summary>
        /// 颁发JWT字符串
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static string IssueJwt(TokenModelJwt tokenModel)
        {
            // 这里就是声明我们的claim
            var claims = new Claim[] {
                    new Claim(ClaimTypes.Name, tokenModel.Name),
                    // new Claim(ClaimTypes.Role, tokenModel.Role),
                    // new Claim(ClaimTypes.Sid,tokenModel.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, tokenModel.Sid.ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, $"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}"),
                    new Claim(JwtRegisteredClaimNames.Nbf,$"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}") ,
                    // 这个就是过期时间，目前是过期1000秒，可自定义，注意JWT有自己的缓冲过期时间
                    new Claim (JwtRegisteredClaimNames.Exp,$"{new DateTimeOffset(DateTime.Now.AddSeconds(1000)).ToUnixTimeSeconds()}"),
                    new Claim(JwtRegisteredClaimNames.Iss,settings.Issuer),
                    new Claim(JwtRegisteredClaimNames.Aud,settings.Audience),
                };

            // 密钥(SymmetricSecurityKey 对安全性的要求，密钥的长度太短会报出异常)
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: settings.Issuer,
                audience: settings.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(1),
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
            catch (Exception e)
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
    }



    /// <summary>
    /// 令牌
    /// </summary>
    public class TokenModelJwt
    {
        /// <summary>
        /// Sid
        /// </summary>
        public long Sid { get; set; }
        /// <summary>
        /// 角色
        /// </summary>
        public string Role { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

    }
}
