using ApiServer.BLL.IBLL;
using ApiServer.Model.Entity;
using Microsoft.AspNetCore.Mvc;

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
    }
}
