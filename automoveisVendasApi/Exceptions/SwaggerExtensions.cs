
using System.Reflection;
using Microsoft.OpenApi.Models;

namespace automoveisVendasApi.Extensions
{
   
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Automóveis Vendas API",
                    Version = "v1",
                    Description = "API REST para gerenciamento de uma concessionária de veículos: " +
                                  "cadastro de clientes, carros e motos, registro de vendas e pagamentos. " +
                                  "Desenvolvida em .NET 9 seguindo Clean Architecture (Domain, Application, " +
                                  "Infrastructure e API), com repositório genérico, tratamento global de " +
                                  "exceções (RFC 7807 / ProblemDetails) e health checks."
                });

                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);

                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
                }
            });

            return services;
        }

        public static WebApplication UseSwaggerDocumentation(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Automóveis Vendas API v1");
            });

            return app;
        }
    }
}