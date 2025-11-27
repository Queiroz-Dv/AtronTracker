using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Shared.Infrastructure.Repositories;

namespace Shared.Infrastructure.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class TransactionalAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // 1. Resolve o gerenciador de transação
            var transactionManager = context.HttpContext.RequestServices.GetRequiredService<ITransactionManager>();

            // 2. Abre o escopo transacional (Abrange toda a execução abaixo)
            using var transaction = transactionManager.CreateScope();
            // 3. Executa a Action (Controller -> Service -> Repository)
            var resultContext = await next();

            // 4. Verifica se houve exceção não tratada ou erro no pipeline
            if (resultContext.Exception == null)
            {
                // Se tudo correu bem, COMITA a transação.
                // Se o método do controller retornar BadRequest, o EF Core não terá salvo nada
                // se o código estiver bem estruturado, mas aqui garantimos o commit do sucesso.
                transaction.Complete();
            }            
            // Se houver Exception, o 'Complete()' não é chamado e o Rollback é automático.
        }
    }
}
