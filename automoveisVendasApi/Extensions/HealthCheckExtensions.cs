using System.Text.Json;
using AutomoveisVendasApi.Infrastructure.Context;
using automoveisVendasApi.HealthChecks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace automoveisVendasApi.Extensions
{
    public static class HealthCheckExtensions
    {
        public static IServiceCollection AddApplicationHealthChecks(this IServiceCollection services)
        {
            services
                .AddHealthChecks()
                .AddCheck<SelfHealthCheck>("self", tags: new[] { "self" })
                .AddDbContextCheck<ApplicationDbContext>("database", tags: new[] { "database", "sqlite" });

            return services;
        }

        public static IEndpointRouteBuilder MapApplicationHealthChecks(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = WriteHealthCheckResponseAsync,
                ResultStatusCodes =
                {
                    [HealthStatus.Healthy] = StatusCodes.Status200OK,
                    [HealthStatus.Degraded] = StatusCodes.Status200OK,
                    [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                }
            });

            return endpoints;
        }

        private static Task WriteHealthCheckResponseAsync(HttpContext context, HealthReport report)
        {
            context.Response.ContentType = "application/json";

            var isDevelopment = context.RequestServices
                .GetRequiredService<IHostEnvironment>()
                .IsDevelopment();

            var payload = new
            {
                status = report.Status.ToString(),
                totalDurationMs = report.TotalDuration.TotalMilliseconds,
                checks = report.Entries.Select(entry => new
                {
                    name = entry.Key,
                    status = entry.Value.Status.ToString(),
                    durationMs = entry.Value.Duration.TotalMilliseconds,
                    description = entry.Value.Description,
                    error = isDevelopment ? entry.Value.Exception?.Message : null
                })
            };

            var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            return context.Response.WriteAsync(json);
        }
    }
}
