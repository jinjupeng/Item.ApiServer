using System.Text.Json;
using System.Text.Json.Nodes;

namespace ApiServer.WebApi.Middlewares
{
    /// <summary>
    /// 敏感数据脱敏工具
    /// </summary>
    internal static class SensitiveDataSanitizer
    {
        private static readonly HashSet<string> SensitiveKeys = new(StringComparer.OrdinalIgnoreCase)
        {
            "password",
            "oldPassword",
            "newPassword",
            "confirmPassword",
            "verificationCode",
            "code",
            "refreshToken",
            "accessToken",
            "token"
        };

        private static readonly string[] SensitivePathKeywords =
        {
            "/api/auth/login",
            "/api/auth/refresh-token",
            "/api/auth/change-password",
            "/api/auth/reset-password",
            "/api/auth/send-reset-code",
            "/api/users/",
        };

        public static string? SanitizeRequestBody(string path, string? body, string? contentType)
        {
            if (string.IsNullOrWhiteSpace(body))
            {
                return body;
            }

            if (ShouldSuppressBody(path))
            {
                return "[REDACTED]";
            }

            return SanitizeJsonBody(body, contentType);
        }

        public static string? SanitizeResponseBody(string path, string? body, string? contentType)
        {
            if (string.IsNullOrWhiteSpace(body))
            {
                return body;
            }

            if (ShouldSuppressBody(path))
            {
                return "[REDACTED]";
            }

            return SanitizeJsonBody(body, contentType);
        }

        private static bool ShouldSuppressBody(string path)
        {
            var normalizedPath = path.ToLowerInvariant();

            if (normalizedPath.Contains("/reset-password") || normalizedPath.Contains("/change-password"))
            {
                return true;
            }

            return SensitivePathKeywords.Any(keyword => normalizedPath.Contains(keyword));
        }

        private static string SanitizeJsonBody(string body, string? contentType)
        {
            if (contentType?.Contains("application/json", StringComparison.OrdinalIgnoreCase) != true)
            {
                return body;
            }

            try
            {
                var node = JsonNode.Parse(body);
                if (node == null)
                {
                    return body;
                }

                SanitizeNode(node);
                return node.ToJsonString();
            }
            catch
            {
                return body;
            }
        }

        private static void SanitizeNode(JsonNode node)
        {
            if (node is JsonObject jsonObject)
            {
                foreach (var property in jsonObject.ToList())
                {
                    if (SensitiveKeys.Contains(property.Key))
                    {
                        jsonObject[property.Key] = "***";
                        continue;
                    }

                    if (property.Value != null)
                    {
                        SanitizeNode(property.Value);
                    }
                }
            }

            if (node is JsonArray jsonArray)
            {
                foreach (var item in jsonArray)
                {
                    if (item != null)
                    {
                        SanitizeNode(item);
                    }
                }
            }
        }
    }
}
