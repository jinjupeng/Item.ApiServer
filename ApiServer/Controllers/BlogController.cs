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
    public class BlogController : ControllerBase
    {
        private readonly IBaseService<Blogs> _baseService;

        public BlogController(IBaseService<Blogs> baseService)
        {
            _baseService = baseService;
        }

        /// <summary>
        /// 我的所有文章首页
        /// </summary>
        /// <param name="author">用户名</param>
        /// <param name="keyword">关键词</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> List(string author, string keyword)
        {
            return Ok();
        }

    }
}
