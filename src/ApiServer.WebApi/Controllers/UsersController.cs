using ApiServer.Application.DTOs.User;
using ApiServer.Application.Interfaces.Services;
using ApiServer.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiServer.WebApi.Controllers
{
    /// <summary>
    /// 用户管理控制器
    /// </summary>
    [Authorize]
    public class UsersController : BaseController
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="query">查询参数</param>
        /// <returns>用户列表</returns>
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] UserQueryDto query)
        {
            var result = await _userService.GetPagedUsersAsync(query);
            return HandleResult(result);
        }

        /// <summary>
        /// 根据ID获取用户详情
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns>用户详情</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(long id)
        {
            var result = await _userService.GetUserByIdAsync(id);
            return HandleResult(result);
        }

        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="dto">创建用户DTO</param>
        /// <returns>创建结果</returns>
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            var result = await _userService.CreateUserAsync(dto);
            return HandleResult(result);
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <param name="dto">更新用户DTO</param>
        /// <returns>更新结果</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(long id, [FromBody] UpdateUserDto dto)
        {
            var result = await _userService.UpdateUserAsync(id, dto);
            return HandleResult(result);
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns>删除结果</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(long id)
        {
            var result = await _userService.DeleteUserAsync(id);
            return HandleResult(result);
        }

        /// <summary>
        /// 更新用户状态
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <param name="status">用户状态</param>
        /// <returns>更新结果</returns>
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateUserStatus(long id, [FromBody] UserStatus status)
        {
            var result = await _userService.UpdateUserStatusAsync(id, status);
            return HandleResult(result);
        }

        /// <summary>
        /// 重置用户密码
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <param name="newPassword">新密码</param>
        /// <returns>重置结果</returns>
        [HttpPost("{id}/reset-password")]
        public async Task<IActionResult> ResetPassword(long id, [FromBody] string newPassword)
        {
            var result = await _userService.ResetPasswordAsync(id, newPassword);
            return HandleResult(result);
        }
    }
}
