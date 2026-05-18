using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ApiServer.WebApi.Configuration
{
    /// <summary>
    /// WebApi 运行时硬化策略
    /// </summary>
    public static class WebApiRuntimePolicy
    {
        public static bool ShouldRequireHttpsMetadata(IHostEnvironment environment, IConfiguration configuration)
        {
            var configured = configuration.GetValue<bool?>("Security:RequireHttpsMetadata");
            if (configured.HasValue)
            {
                return configured.Value;
            }

            return !environment.IsDevelopment();
        }

        public static bool ShouldRunAutoInitialization(IHostEnvironment environment, IConfiguration configuration)
        {
            var configured = configuration.GetValue<bool?>("Database:AutoMigrateOnStartup");
            if (configured.HasValue)
            {
                return configured.Value;
            }

            return environment.IsDevelopment();
        }

        public static string[] GetAllowedCorsOrigins(IConfiguration configuration)
        {
            var configured = configuration["Cors:AllowedOrigins"];
            if (string.IsNullOrWhiteSpace(configured))
            {
                return Array.Empty<string>();
            }

            return configured
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(origin => !string.IsNullOrWhiteSpace(origin))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }
    }
}
