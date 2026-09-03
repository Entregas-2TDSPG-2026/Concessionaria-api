
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace automoveisVendasApi.HealthChecks
{
 
    public class SelfHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(HealthCheckResult.Healthy("A API está em execução."));
        }
    }
}