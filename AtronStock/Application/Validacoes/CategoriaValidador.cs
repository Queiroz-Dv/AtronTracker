using AtronStock.Application.DTO.Request; 
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources.AtronStock; 
using Shared.Domain.ValueObjects;
using Shared.Extensions;

namespace AtronStock.Application.Validacoes
{
    public class CategoriaValidador : IValidador<CategoriaRequest>
    {
        public IList<NotificationMessage> Validar(CategoriaRequest entity)
        {
            var context = new NotificationBag();

            if (entity.Codigo.IsNullOrEmpty())
            {
                context.AdicionarErro(CategoriaResource.ErroCodigoObrigatorio);
            }
            else if (entity.Codigo.Length > 25)
            {
                context.AdicionarErro(CategoriaResource.ErroCodigoTamanho);
            }

            if (entity.Descricao.IsNullOrEmpty())
            {
                context.AdicionarErro(CategoriaResource.ErroDescricaoObrigatoria);
            }
            else if (entity.Descricao.Length > 50)
            {
                context.AdicionarErro(CategoriaResource.ErroDescricaoTamanho);
            }

            if (entity.Status.GetDescription().IsNullOrEmpty())
            {
                context.AdicionarErro(CategoriaResource.ErroStatusObrigatorio);
            }

            return context.Messages.ToList();
        }
    }
}