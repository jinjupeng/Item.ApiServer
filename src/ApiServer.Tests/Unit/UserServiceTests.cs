using ApiServer.Application.Services;
using ApiServer.Domain.Entities;
using ApiServer.Infrastructure.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ApiServer.Tests.Unit.Services
{
    public class UserServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            // 使用内存数据库进行测试
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _userService = new UserService(_context);
        }

        [Fact]
        public async Task GetByUsernameAsync_WithValidUsername_ShouldReturnUser()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Username = "testuser",
                Password = "password123",
                Nickname = "Test User",
                Email = "test@example.com",
                Enabled = true,
                CreateTime = DateTime.Now
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userService.GetByUsernameAsync("testuser");

            // Assert
            result.Should().NotBeNull();
            result.Username.Should().Be("testuser");
            result.Email.Should().Be("test@example.com");
        }

        [Fact]
        public async Task GetByUsernameAsync_WithInvalidUsername_ShouldReturnNull()
        {
            // Act
            var result = await _userService.GetByUsernameAsync("nonexistent");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task UsernameExistsAsync_WithExistingUsername_ShouldReturnTrue()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Username = "existinguser",
                Password = "password123",
                CreateTime = DateTime.Now
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userService.UsernameExistsAsync("existinguser");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task UsernameExistsAsync_WithNonExistingUsername_ShouldReturnFalse()
        {
            // Act
            var result = await _userService.UsernameExistsAsync("nonexisting");

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task AddAsync_WithValidUser_ShouldCreateUser()
        {
            // Arrange
            var user = new User
            {
                Username = "newuser",
                Password = "password123",
                Nickname = "New User",
                Email = "new@example.com",
                Enabled = true
            };

            // Act
            var result = await _userService.AddAsync(user);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.Username.Should().Be("newuser");
            result.CreateTime.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));

            // 验证密码是否已加密
            result.Password.Should().NotBe("password123");
        }

        [Fact]
        public async Task ValidateUserAsync_WithCorrectCredentials_ShouldReturnUser()
        {
            // Arrange
            var user = new User
            {
                Username = "validuser",
                Password = "password123",
                Enabled = true,
                CreateTime = DateTime.Now
            };

            await _userService.AddAsync(user); // 这会加密密码

            // Act
            var result = await _userService.ValidateUserAsync("validuser", "password123");

            // Assert
            result.Should().NotBeNull();
            result.Username.Should().Be("validuser");
        }

        [Fact]
        public async Task ValidateUserAsync_WithIncorrectPassword_ShouldReturnNull()
        {
            // Arrange
            var user = new User
            {
                Username = "validuser",
                Password = "password123",
                Enabled = true,
                CreateTime = DateTime.Now
            };

            await _userService.AddAsync(user);

            // Act
            var result = await _userService.ValidateUserAsync("validuser", "wrongpassword");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task ValidateUserAsync_WithDisabledUser_ShouldReturnNull()
        {
            // Arrange
            var user = new User
            {
                Username = "disableduser",
                Password = "password123",
                Enabled = false,
                CreateTime = DateTime.Now
            };

            await _userService.AddAsync(user);

            // Act
            var result = await _userService.ValidateUserAsync("disableduser", "password123");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task SetUserStatusAsync_WithValidUser_ShouldUpdateStatus()
        {
            // Arrange
            var user = new User
            {
                Username = "statususer",
                Password = "password123",
                Enabled = true,
                CreateTime = DateTime.Now
            };

            var addedUser = await _userService.AddAsync(user);

            // Act
            var result = await _userService.SetUserStatusAsync(addedUser.Id, false);

            // Assert
            result.Should().NotBeNull();
            result.success.Should().BeTrue();
            result.msg.Should().Contain("禁用成功");

            // 验证用户状态是否已更新
            var updatedUser = await _userService.GetByIdAsync(addedUser.Id);
            updatedUser.Enabled.Should().BeFalse();
        }

        [Fact]
        public async Task ChangePasswordAsync_WithCorrectOldPassword_ShouldSucceed()
        {
            // Arrange
            var user = new User
            {
                Username = "changepassuser",
                Password = "oldpassword",
                Enabled = true,
                CreateTime = DateTime.Now
            };

            var addedUser = await _userService.AddAsync(user);

            // Act
            var result = await _userService.ChangePasswordAsync(addedUser.Id, "oldpassword", "newpassword");

            // Assert
            result.Should().NotBeNull();
            result.success.Should().BeTrue();
            result.msg.Should().Contain("修改成功");
        }

        [Fact]
        public async Task ChangePasswordAsync_WithIncorrectOldPassword_ShouldFail()
        {
            // Arrange
            var user = new User
            {
                Username = "changepassuser",
                Password = "oldpassword",
                Enabled = true,
                CreateTime = DateTime.Now
            };

            var addedUser = await _userService.AddAsync(user);

            // Act
            var result = await _userService.ChangePasswordAsync(addedUser.Id, "wrongoldpassword", "newpassword");

            // Assert
            result.Should().NotBeNull();
            result.success.Should().BeFalse();
            result.msg.Should().Contain("原密码错误");
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}