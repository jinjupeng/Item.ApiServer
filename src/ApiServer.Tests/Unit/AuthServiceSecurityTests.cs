using ApiServer.Application.DTOs.Auth;
using ApiServer.Application.Interfaces;
using ApiServer.Application.Interfaces.Repositories;
using ApiServer.Application.Interfaces.Services;
using ApiServer.Application.Services;
using ApiServer.Domain.Entities;
using ApiServer.Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace ApiServer.Tests.Unit;

public class AuthServiceSecurityTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IMenuRepository> _menuRepository = new();
    private readonly Mock<IAuthSecurityService> _authSecurityService = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly IConfiguration _configuration;

    public AuthServiceSecurityTests()
    {
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtSettings:SecretKey"] = "YourSuperSecretKeyThatIsAtLeast32CharactersLong123456",
                ["JwtSettings:Issuer"] = "ApiServer",
                ["JwtSettings:Audience"] = "ApiServer"
            })
            .Build();
    }

    [Fact]
    public async Task RefreshTokenAsync_ShouldAcceptTokenIssuedDuringLogin()
    {
        var user = CreateEnabledUser();

        _userRepository.Setup(x => x.GetByUsernameAsync(user.Name)).ReturnsAsync(user);
        _userRepository.Setup(x => x.GetByIdAsync(user.Id)).ReturnsAsync(user);
        _userRepository.Setup(x => x.GetUserWithRolesAsync(user.Id)).ReturnsAsync(user);
        _userRepository.Setup(x => x.GetUserPermissionsAsync(user.Id)).ReturnsAsync(Array.Empty<Permission>());
        _authSecurityService.SetupSequence(x => x.IssueRefreshTokenAsync(user.Id))
            .ReturnsAsync("issued-refresh-token")
            .ReturnsAsync("rotated-refresh-token");
        _authSecurityService.Setup(x => x.RedeemRefreshTokenAsync("issued-refresh-token"))
            .ReturnsAsync(user.Id);

        var service = CreateService();

        var loginResult = await service.LoginAsync(new LoginDto
        {
            Username = user.Name,
            Password = "Password123!"
        });

        loginResult.Success.Should().BeTrue();
        loginResult.Data.Should().NotBeNull();

        var refreshResult = await service.RefreshTokenAsync(new RefreshTokenDto
        {
            RefreshToken = loginResult.Data!.RefreshToken
        });

        refreshResult.Success.Should().BeTrue();
        refreshResult.Data.Should().NotBeNull();
        refreshResult.Data!.AccessToken.Should().NotBeNullOrWhiteSpace();
        refreshResult.Data.RefreshToken.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task ResetPasswordAsync_ShouldRejectInvalidVerificationCode()
    {
        var user = CreateEnabledUser();

        _userRepository.Setup(x => x.GetByUsernameAsync(user.Name)).ReturnsAsync(user);
        _userRepository.Setup(x => x.GetByEmailAsync(user.Email!)).ReturnsAsync(user);
        _authSecurityService.Setup(x => x.ValidatePasswordResetCodeAsync(user.Name, "WRONG", true))
            .ReturnsAsync(false);

        var service = CreateService();

        var result = await service.ResetPasswordAsync(new ResetPasswordDto
        {
            UsernameOrEmail = user.Name,
            VerificationCode = "WRONG",
            NewPassword = "NewPassword123!"
        });

        result.Success.Should().BeFalse();
        _unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    private AuthService CreateService()
    {
        return new AuthService(
            _userRepository.Object,
            _menuRepository.Object,
            _authSecurityService.Object,
            _configuration,
            _unitOfWork.Object);
    }

    private static User CreateEnabledUser()
    {
        return new User
        {
            Id = 1,
            Name = "admin",
            Password = BCrypt.Net.BCrypt.HashPassword("Password123!"),
            Status = UserStatus.Enabled,
            Email = "admin@example.com",
            NickName = "Admin"
        };
    }
}
