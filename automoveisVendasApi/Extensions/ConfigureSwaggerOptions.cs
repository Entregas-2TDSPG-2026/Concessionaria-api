using System.Reflection;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace automoveisVendasApi.Extensions
{
    public sealed class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;

        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) => _provider = provider;

        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                var info = new OpenApiInfo
                {
                    Title = "Automóveis Vendas API",
                    Version = description.ApiVersion.ToString(),
                    Description = "API REST para gerenciamento de uma concessionária de veículos: " +
                                  "cadastro de clientes, carros e motos, registro de vendas e pagamentos. " +
                                  "Desenvolvida em .NET 9 seguindo Clean Architecture."
                };

                if (description.IsDeprecated)
                {
                    info.Description += " **ATENÇÃO: esta versão (1.0) está OBSOLETA.** " +
                                        "A listagem de vendas devolve o array completo, sem paginação. " +
                                        "Ela continua funcionando, mas migre para a versão 2.0 (envelope paginado).";
                }
                else
                {
                    info.Description += " Versão atual (2.0): a listagem de vendas é paginada " +
                                        "(page, pageSize) e devolve um envelope com totais.";
                }

                options.SwaggerDoc(description.GroupName, info);
            }

            var xmlPath = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
            if (File.Exists(xmlPath))
                options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
        }
    }
}
