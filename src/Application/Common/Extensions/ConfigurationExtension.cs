using Microsoft.Extensions.Configuration;

namespace Application.Common.Extensions;

public static class ConfigurationExtension
{
    public static bool IsTestEnvironment(this IConfiguration configuration)
    {
        return configuration["IsTestEnvironment"]?.ToLower() == "true";
    }
}