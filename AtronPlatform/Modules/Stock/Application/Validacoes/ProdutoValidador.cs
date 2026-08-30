#nullable enable

using AtronStock.Application.DTO.Request;
using AtronStock.Application.Resources;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using Shared.Extensions;

namespace AtronStock.Application.Validacoes
{
    public sealed class ProdutoValidador :
        IValidador<ProdutoRequest>,
        IValidador<ProdutoAtualizacaoRequest>
    {
        public IEnumerable<NotificationMessage> Validar(ProdutoRequest request)
            => ValidarCampos(
                request.Codigo,
                request.Descricao,
                request.DataAquisicao,
                request.PrecoUnitario);

        public IEnumerable<NotificationMessage> Validar(ProdutoAtualizacaoRequest request)
            => ValidarCampos(
                null,
                request.Descricao,
                request.DataAquisicao,
                request.PrecoUnitario);

        private static IEnumerable<NotificationMessage> ValidarCampos(
            string? codigo,
            string descricao,
            DateTime dataAquisicao,
            decimal precoUnitario)
        {
            var context = new NotificationBag();
            ValidarCodigo(codigo, context);
            ValidarDescricao(descricao, context);

            if (dataAquisicao == default)
                context.AdicionarErro(ProdutoResource.ErroDataAquisicaoObrigatoria);

            if (precoUnitario <= 0)
                context.AdicionarErro(ProdutoResource.ErroPrecoProduto);

            return [.. context.Messages];
        }

        private static void ValidarCodigo(string? codigo, NotificationBag context)
        {
            if (codigo is null) return;

            if (codigo.IsNullOrEmpty())
            {
                context.AdicionarErro(ProdutoResource.ErroCodigoObrigatorio);
                return;
            }

            if (codigo.Length > 25)
            {
                context.AdicionarErro(ProdutoResource.ErroCodigoLimiteMaximoDeCaractere);
                return;
            }

            if (codigo.Length < 3)
                context.AdicionarErro(ProdutoResource.ErroCodigoLimiteMinimoDeCaractere);
        }

        private static void ValidarDescricao(string descricao, NotificationBag context)
        {
            if (descricao.IsNullOrEmpty())
            {
                context.AdicionarErro(ProdutoResource.ErroDescricaoObrigatoria);
                return;
            }

            if (descricao.Trim().Length > 50)
            {
                context.AdicionarErro(ProdutoResource.ErroDescricaoLimiteMaximoCaractere);
                return;
            }

            if (descricao.Trim().Length < 5)
                context.AdicionarErro(ProdutoResource.ErroDescricaoLimiteMinimoCaractere);
        }
    }
}
