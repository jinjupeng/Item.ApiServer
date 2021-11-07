using ApiServer.BLL.BLL;
using ApiServer.Models.Model.MsgModel;
using Moq;
using Xunit;

namespace Test.Controller
{
    public class JwtAuthControllerTest
    {
        private Mock<IJwtAuthService> _jwtAuthService;

        public JwtAuthControllerTest()
        {
            _jwtAuthService = new Mock<IJwtAuthService>();
        }

        [Fact]
        public void LoginTest()
        {
            _jwtAuthService.Setup(_ => _.Login("admin", "123456")).Returns(
                new MsgModel {
                    code = 200,
                    message = "登录成功！"
                });

            var result = _jwtAuthService.Object.Login("admin", "123456");
            Assert.True(result.isok);
        }
    }
}
