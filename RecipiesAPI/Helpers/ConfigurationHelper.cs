namespace RecipiesAPI.Helpers
{
    /// <summary>
    /// Helper class for safe configuration value parsing
    /// </summary>
    public static class ConfigurationHelper
    {
        /// <summary>
        /// Safely gets an integer configuration value with validation
        /// </summary>
        /// <param name="configuration">The configuration instance</param>
        /// <param name="key">The configuration key</param>
        /// <param name="defaultValue">Optional default value if key is missing</param>
        /// <returns>The parsed integer value</returns>
        /// <exception cref="InvalidOperationException">Thrown when the key is missing and no default is provided, or when value is not a valid integer</exception>
        public static int GetRequiredInt(this IConfiguration configuration, string key, int? defaultValue = null)
        {
            var value = configuration[key];

            if (string.IsNullOrWhiteSpace(value))
            {
                if (defaultValue.HasValue)
                {
                    return defaultValue.Value;
                }
                throw new InvalidOperationException($"Configuration key '{key}' is required but was not found.");
            }

            if (!int.TryParse(value, out var result))
            {
                throw new InvalidOperationException($"Configuration key '{key}' value '{value}' is not a valid integer.");
            }

            return result;
        }

        /// <summary>
        /// Safely gets a string configuration value with validation
        /// </summary>
        /// <param name="configuration">The configuration instance</param>
        /// <param name="key">The configuration key</param>
        /// <param name="defaultValue">Optional default value if key is missing</param>
        /// <returns>The configuration value</returns>
        /// <exception cref="InvalidOperationException">Thrown when the key is missing and no default is provided</exception>
        public static string GetRequiredString(this IConfiguration configuration, string key, string? defaultValue = null)
        {
            var value = configuration[key];

            if (string.IsNullOrWhiteSpace(value))
            {
                if (defaultValue != null)
                {
                    return defaultValue;
                }
                throw new InvalidOperationException($"Configuration key '{key}' is required but was not found.");
            }

            return value;
        }

        /// <summary>
        /// Validates that all required JWT settings are present
        /// </summary>
        /// <param name="configuration">The configuration instance</param>
        /// <exception cref="InvalidOperationException">Thrown when any required setting is missing</exception>
        public static void ValidateJwtSettings(this IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");

            GetRequiredString(jwtSettings, "Secret");
            GetRequiredString(jwtSettings, "Issuer");
            GetRequiredString(jwtSettings, "Audience");
            GetRequiredInt(jwtSettings, "TokenExpirationMinutes");
            GetRequiredInt(jwtSettings, "RefreshTokenExpirationDays");
        }
    }
}
