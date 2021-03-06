﻿using ApiServer.BLL.IBLL;
using ApiServer.Model.Entity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ApiServer.Controllers
{
    [Route("api/[controller]")]
    public class BlogController : BaseController
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
