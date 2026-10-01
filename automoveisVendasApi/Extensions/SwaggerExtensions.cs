using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace automoveisVendasApi.Extensions
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            // Os documentos (um por versão) são criados em ConfigureSwaggerOptions
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            return services;
        }

        public static WebApplication UseSwaggerDocumentation(this WebApplication app)
        {
            var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                foreach (var d in provider.ApiVersionDescriptions.OrderByDescending(x => x.ApiVersion))
                {
                    var nome = $"Automóveis Vendas API {d.GroupName}" + (d.IsDeprecated ? " (obsoleta)" : "");
                    options.SwaggerEndpoint($"/swagger/{d.GroupName}/swagger.json", nome);
                }
            });

            return app;
        }
    }
}