using System.Globalization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace automoveisVendasApi.Extensions
{
    public static class RateLimitingExtensions
    {
        public const string EscritaPolicy = "escrita";
        public const string LeituraPolicy = "leitura";

        public const int EscritaLimit = 10;
        public static readonly TimeSpan EscritaWindow = TimeSpan.FromMinutes(1);

        public const int LeituraLimit = 60;
        public static readonly TimeSpan LeituraWindow = TimeSpan.FromMinutes(1);

        public static IServiceCollection AddRateLimitingConfiguration(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.AddPolicy(EscritaPolicy, ctx => FixedWindowPorIp(ctx, EscritaPolicy, EscritaLimit, EscritaWindow));
                options.AddPolicy(LeituraPolicy, ctx => FixedWindowPorIp(ctx, LeituraPolicy, LeituraLimit, LeituraWindow));

                options.OnRejected = async (context, cancellationToken) =>
                {
                    var http = context.HttpContext;

                    var retryAfterSeconds = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter)
                        ? (int)Math.Ceiling(retryAfter.TotalSeconds)
                        : (int)EscritaWindow.TotalSeconds;

                    http.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    http.Response.Headers.RetryAfter = retryAfterSeconds.ToString(CultureInfo.InvariantCulture);

                    var problem = new ProblemDetails
                    {
                        Status = StatusCodes.Status429TooManyRequests,
                        Title = "Muitas requisições",
                        Type = "https://httpstatuses.io/429",
                        Detail = $"Limite de requisições excedido. Tente novamente em {retryAfterSeconds} segundo(s).",
                        Instance = http.Request.Path
                    };
                    problem.Extensions["traceId"] = http.TraceIdentifier;
                    problem.Extensions["retryAfterSeconds"] = retryAfterSeconds;

                    await http.Response.WriteAsJsonAsync(
                        problem, (System.Text.Json.JsonSerializerOptions?)null, "application/problem+json", cancellationToken);
                };
            });

            return services;
        }

        private static RateLimitPartition<string> FixedWindowPorIp(HttpContext ctx, string policy, int limit, TimeSpan window)
        {
            var ip = ctx.Connection.RemoteIpAddress?.ToString() ?? "desconhecido";

            return RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: $"{policy}:{ip}",
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = limit,
                    Window = window,
                    QueueLimit = 0,
                    AutoReplenishment = true
                });
        }
    }
}
