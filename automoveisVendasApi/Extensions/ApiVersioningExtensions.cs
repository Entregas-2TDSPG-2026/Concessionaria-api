using Asp.Versioning;

namespace automoveisVendasApi.Extensions
{
    public static class ApiVersioningExtensions
    {
        public static IServiceCollection AddApiVersioningConfiguration(this IServiceCollection services)
        {
            services
                .AddApiVersioning(options =>
                {
                    options.DefaultApiVersion = new ApiVersion(2, 0);
                    options.AssumeDefaultVersionWhenUnspecified = true;   // sem versão -> 2.0
                    options.ReportApiVersions = true;                      // api-supported-versions / api-deprecated-versions

                    options.ApiVersionReader = ApiVersionReader.Combine(
                        new QueryStringApiVersionReader("api-version"),    // ?api-version=1.0
                        new HeaderApiVersionReader("X-Api-Version"),       // X-Api-Version: 1.0
                        new UrlSegmentApiVersionReader());                 // /api/v1/vendas (recomendado)
                })
                .AddMvc()
                .AddApiExplorer(options =>
                {
                    options.GroupNameFormat = "'v'VVVV";                   // v1.0, v2.0
                    options.SubstituteApiVersionInUrl = true;
                });

            return services;
        }
    }
}