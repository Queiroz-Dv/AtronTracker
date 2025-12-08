using AtronStock.Application.DTO.Request;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources.AtronStock;
using Shared.Domain.ValueObjects;
using Shared.Extensions;

namespace AtronStock.Application.Validacoes
{
    public class ProdutoValidador : IValidador<ProdutoRequest>
    {
        public IList<NotificationMessage> Validar(ProdutoRequest entity)
        {
            var context = new NotificationBag();
            ValidaCodigo(entity, context);
            ValidaDescricao(entity, context);

            if (entity.Preco <= 0)
            {
                context.AdicionarErro(ProdutoResource.ErroPrecoProduto);
            }

            ValidaRelacionamentos(entity, context);

            return [.. context.Messages];
        }

        private static void ValidaCodigo(ProdutoRequest entity, NotificationBag context)
        {
            if (entity.Codigo.IsNullOrEmpty())
            {
                context.AdicionarErro(ProdutoResource.ErroCodigoObrigatorio);
            }
            else if (entity.Codigo.Length > 25)
            {
                context.AdicionarErro(ProdutoResource.ErroCodigoLimiteMaximoDeCaractere);
            }
            else if (entity.Codigo.Length < 3)
            {
                context.AdicionarErro(ProdutoResource.ErroCodigoLimiteMinimoDeCaractere);
            }
        }

        private static void ValidaDescricao(ProdutoRequest entity, NotificationBag context)
        {
            if (entity.Descricao.IsNullOrEmpty())
            {
                context.AdicionarErro(ProdutoResource.ErroDescricaoObrigatoria);
            }
            else if (entity.Descricao.Length > 100)
            {
                context.AdicionarErro(ProdutoResource.ErroDescricaoLimiteMaximoCaractere);
            }
            else if (entity.Descricao.Length < 5)
            {
                context.AdicionarErro(ProdutoResource.ErroDescricaoLimiteMinimoCaractere);
            }
        }

        private static void ValidaRelacionamentos(ProdutoRequest entity, NotificationBag context)
        {
            if (entity.CategoriaCodigos == null || entity.CategoriaCodigos.Count == 0)
            {
                context.AdicionarErro(ProdutoResource.ErroProdutoSemCategoria);
            }

            if (entity.FornecedoresCodigos == null || entity.FornecedoresCodigos.Count == 0)
            {
                context.AdicionarErro(ProdutoResource.ErroProdutoSemFornecedor);
            }
        }
    }
}