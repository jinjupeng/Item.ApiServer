using ApiServer.Application.DTOs.User;
using ApiServer.Application.Interfaces;
using ApiServer.Application.Interfaces.Repositories;
using ApiServer.Application.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace ApiServer.Tests.Unit;

public class UserServiceExceptionTests
{
    [Fact]
    public async Task CreateUserAsync_ShouldBubbleUnexpectedException()
    {
        var userRepository = new Mock<IUserRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        userRepository.Setup(x => x.IsUsernameExistsAsync("admin", null)).ReturnsAsync(false);
        userRepository.Setup(x => x.IsEmailExistsAsync("admin@example.com", null)).ReturnsAsync(false);
        userRepository.Setup(x => x.AddAsync(It.IsAny<ApiServer.Domain.Entities.User>()))
            .ThrowsAsync(new InvalidOperationException("db failure"));

        var service = new UserService(userRepository.Object, unitOfWork.Object);

        var action = () => service.CreateUserAsync(new CreateUserDto
        {
            Username = "admin",
            Password = "Password123!",
            Nickname = "Admin",
            Email = "admin@example.com"
        });

        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("db failure");
    }
}
