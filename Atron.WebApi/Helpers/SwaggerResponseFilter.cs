using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Atron.WebApi.Helpers
{
    public class SwaggerResponseFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            //if (context.MethodInfo.ReturnType == typeof(ActionResult))
            //{
            //    operation.Responses.Add("200", new OpenApiResponse { Description = "Operação realizada com sucesso." });
            //    operation.Responses.Add("400", new OpenApiResponse { Description = "Erro na requisição." });
            //    operation.Responses.Add("401", new OpenApiResponse { Description = "Não autorizado." });
            //}
            operation.Responses.TryAdd("400", new OpenApiResponse { Description = "Erro na requisição." });
            operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Não autorizado." });
            operation.Responses.TryAdd("500", new OpenApiResponse { Description = "Erro interno do servidor." });
        }
    }
}