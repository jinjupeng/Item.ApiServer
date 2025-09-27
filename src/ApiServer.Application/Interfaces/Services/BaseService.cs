namespace ApiServer.Application.Interfaces.Services
{
    /// <summary>
    /// 基础服务类
    /// </summary>
    public abstract class BaseService
    {
        /// <summary>
        /// 获取当前时间
        /// </summary>
        /// <returns>当前时间</returns>
        protected DateTime GetCurrentTime()
        {
            return DateTime.Now;
        }

        /// <summary>
        /// 生成随机字符串
        /// </summary>
        /// <param name="length">长度</param>
        /// <returns>随机字符串</returns>
        protected string GenerateRandomString(int length = 8)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// 验证邮箱格式
        /// </summary>
        /// <param name="email">邮箱</param>
        /// <returns>是否有效</returns>
        protected bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 验证手机号格式
        /// </summary>
        /// <param name="phone">手机号</param>
        /// <returns>是否有效</returns>
        protected bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            // 简单的手机号验证，实际应用中应该根据需求调整
            return System.Text.RegularExpressions.Regex.IsMatch(phone, @"^1[3-9]\d{9}$");
        }
    }
}