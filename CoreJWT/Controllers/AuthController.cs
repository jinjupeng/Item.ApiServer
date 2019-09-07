using Common.JWT;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CoreJWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Get(string userName, string pwd)
        {
            TokenModelJwt tokenModel = new TokenModelJwt();
            
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(pwd))
            {
                tokenModel.Name = userName;
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
    }
}
