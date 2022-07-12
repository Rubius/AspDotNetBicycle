using Microsoft.Extensions.DependencyInjection;

namespace Localization;

public static class DependencyInjection
{
    public static IServiceCollection AddAppLocalization(this IServiceCollection services)
    {
        return services.AddLocalization(options => options.ResourcesPath = "Resources");
    }
}
