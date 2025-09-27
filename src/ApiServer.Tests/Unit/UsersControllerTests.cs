using ApiServer.Controllers;
using ApiServer.Application.Interfaces;
using ApiServer.Domain.Entities;
using ApiServer.Models.Model.MsgModel;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ApiServer.Tests.Unit.Controllers
{
    public class UsersControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _controller = new UsersController(_mockUserService.Object);
        }

        [Fact]
        public async Task GetUser_WithValidId_ShouldReturnSuccessResult()
        {
            // Arrange
            var userId = 1L;
            var user = new User
            {
                Id = userId,
                Username = "testuser",
                Email = "test@example.com"
            };

            _mockUserService.Setup(x => x.GetByIdAsync(userId))
                           .ReturnsAsync(user);

            // Act
            var result = await _controller.GetUser(userId);

            // Assert
            result.Should().NotBeNull();
            result.success.Should().BeTrue();
            result.data.Should().NotBeNull();
        }

        [Fact]
        public async Task GetUser_WithInvalidId_ShouldReturnFailResult()
        {
            // Arrange
            var userId = 999L;
            _mockUserService.Setup(x => x.GetByIdAsync(userId))
                           .ReturnsAsync((User)null);

            // Act
            var result = await _controller.GetUser(userId);

            // Assert
            result.Should().NotBeNull();
            result.success.Should().BeFalse();
            result.msg.Should().Contain("用户不存在");
        }

        [Fact]
        public async Task DeleteUser_WithValidId_ShouldReturnSuccessResult()
        {
            // Arrange
            var userId = 1L;
            _mockUserService.Setup(x => x.DeleteAsync(userId))
                           .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            result.Should().NotBeNull();
            result.success.Should().BeTrue();
            result.msg.Should().Contain("删除成功");
        }

        [Fact]
        public async Task DeleteUser_WithInvalidId_ShouldReturnFailResult()
        {
            // Arrange
            var userId = 999L;
            _mockUserService.Setup(x => x.DeleteAsync(userId))
                           .ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            result.Should().NotBeNull();
            result.success.Should().BeFalse();
            result.msg.Should().Contain("不存在或删除失败");
        }

        [Fact]
        public async Task SetUserStatus_ShouldCallServiceMethod()
        {
            // Arrange
            var userId = 1L;
            var enabled = true;
            var expectedResult = ApiResult.Success("用户启用成功");

            _mockUserService.Setup(x => x.SetUserStatusAsync(userId, enabled))
                           .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.SetUserStatus(userId, enabled);

            // Assert
            result.Should().NotBeNull();
            result.success.Should().BeTrue();
            _mockUserService.Verify(x => x.SetUserStatusAsync(userId, enabled), Times.Once);
        }

        [Fact]
        public async Task ChangePassword_WithValidModel_ShouldReturnSuccessResult()
        {
            // Arrange
            var userId = 1L;
            var model = new ChangePasswordModel
            {
                OldPassword = "oldpass",
                NewPassword = "newpass"
            };
            var expectedResult = ApiResult.Success("密码修改成功");

            _mockUserService.Setup(x => x.ChangePasswordAsync(userId, model.OldPassword, model.NewPassword))
                           .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.ChangePassword(userId, model);

            // Assert
            result.Should().NotBeNull();
            result.success.Should().BeTrue();
            _mockUserService.Verify(x => x.ChangePasswordAsync(userId, model.OldPassword, model.NewPassword), Times.Once);
        }

        [Fact]
        public async Task ChangePassword_WithInvalidModel_ShouldReturnFailResult()
        {
            // Arrange
            var userId = 1L;
            var model = new ChangePasswordModel
            {
                OldPassword = "",
                NewPassword = "newpass"
            };

            // Act
            var result = await _controller.ChangePassword(userId, model);

            // Assert
            result.Should().NotBeNull();
            result.success.Should().BeFalse();
            result.msg.Should().Contain("密码信息不完整");
            _mockUserService.Verify(x => x.ChangePasswordAsync(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void Constructor_WithNullUserService_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new UsersController(null));
        }
    }
}