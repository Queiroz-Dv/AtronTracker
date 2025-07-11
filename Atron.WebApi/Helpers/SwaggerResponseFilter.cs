using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Atron.WebApi.Helpers
{
    public class SwaggerResponseFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Responses.TryAdd("400", new OpenApiResponse { Description = "Erro na requisição." });
            operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Não autorizado." });
            operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Acesso negado." });
            operation.Responses.TryAdd("500", new OpenApiResponse { Description = "Erro interno do servidor." });
        }
    }
}