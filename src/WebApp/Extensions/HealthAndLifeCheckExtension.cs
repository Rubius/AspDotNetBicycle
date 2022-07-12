using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace WebApp.Extensions;

public static class HealthAndLifeCheckExtension
{
    public static void UseHealthAndLifeCheck(this WebApplication app)
    {
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHealthChecks("/health-ready", new HealthCheckOptions
            {
                AllowCachingResponses = false,
                ResultStatusCodes =
                {
                    [HealthStatus.Healthy] = StatusCodes.Status200OK,
                    [HealthStatus.Degraded] = StatusCodes.Status503ServiceUnavailable,
                    [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                }
            });

            endpoints.MapHealthChecks("/health-live", new HealthCheckOptions
            {
                Predicate = _ => false
            });

            endpoints.MapControllers();
        });
    }
}