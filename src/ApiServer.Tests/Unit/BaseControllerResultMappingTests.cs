using ApiServer.Shared.Common;
using ApiServer.WebApi.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace ApiServer.Tests.Unit;

public class BaseControllerResultMappingTests
{
    [Fact]
    public void HandleResult_ShouldReturnNotFound_WhenResultCodeIs404()
    {
        var controller = new TestController();

        var result = controller.InvokeHandleResult(ApiResult.Failed("用户不存在", 404));

        var objectResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        objectResult.Value.Should().BeEquivalentTo(new
        {
            Success = false,
            Code = 404,
            Message = "用户不存在"
        });
    }

    [Fact]
    public void HandleResult_ShouldReturnConflict_WhenResultCodeIs409()
    {
        var controller = new TestController();

        var result = controller.InvokeHandleResult(ApiResult.Failed("用户名已存在", 409));

        var objectResult = result.Should().BeOfType<ConflictObjectResult>().Subject;
        objectResult.Value.Should().BeEquivalentTo(new
        {
            Success = false,
            Code = 409,
            Message = "用户名已存在"
        });
    }

    private sealed class TestController : BaseController
    {
        public IActionResult InvokeHandleResult(ApiResult result) => HandleResult(result);
    }
}
