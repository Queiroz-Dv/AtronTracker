using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shared.Infrastructure.Repositories;

namespace Shared.Infrastructure.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class TransactionalAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var transactionManager = context.HttpContext.RequestServices.GetRequiredService<ITransactionManager>();

            using var transaction = transactionManager.CreateScope();
            var resultContext = await next();

            if (resultContext.Exception == null &&
                RespostaFoiBemSucedida(resultContext))
                transaction.Complete();
        }

        private static bool RespostaFoiBemSucedida(ActionExecutedContext context)
        {
            var statusCode = context.Result switch
            {
                StatusCodeResult resultado => resultado.StatusCode,
                ObjectResult resultado => resultado.StatusCode ?? context.HttpContext.Response.StatusCode,
                ContentResult resultado => resultado.StatusCode ?? context.HttpContext.Response.StatusCode,
                RedirectResult or LocalRedirectResult or RedirectToActionResult or RedirectToRouteResult
                    => StatusCodes.Status300MultipleChoices,
                ChallengeResult => StatusCodes.Status401Unauthorized,
                ForbidResult => StatusCodes.Status403Forbidden,
                _ => context.HttpContext.Response.StatusCode
            };

            return statusCode >= StatusCodes.Status200OK
                && statusCode < StatusCodes.Status300MultipleChoices;
        }
    }
}
