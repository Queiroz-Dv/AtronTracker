using AtronStock.Application.DTO.Request;
using AtronStock.Application.Resources;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;

namespace AtronStock.Application.Validacoes;

public sealed class GeracaoProdutosLoteValidador(
    IValidador<ProdutoAtualizacaoRequest> produtoValidador)
{
    private readonly IValidador<ProdutoAtualizacaoRequest> _produtoValidador = produtoValidador;

    public IEnumerable<NotificationMessage> Validar(GeracaoProdutosLoteCommand command)
    {
        var notificacoes = new NotificationBag();
        var mensagensProduto = _produtoValidador.Validar(new ProdutoAtualizacaoRequest
        {
            Descricao = command.Descricao,
            DescricaoComplementar = command.DescricaoComplementar,
            DataAquisicao = command.DataAquisicao,
            PrecoUnitario = command.PrecoUnitario,
            CategoriaCodigos = [.. command.CategoriaCodigos]
        });

        foreach (var mensagem in mensagensProduto)
            notificacoes.Adicionar(mensagem);

        var codigoBase = command.CodigoBase?.Trim() ?? string.Empty;
        if (codigoBase.Length == 0)
            notificacoes.AdicionarErro(ProdutoResource.ErroCodigoBaseObrigatorio);

        if (command.Quantidade < 1)
            notificacoes.AdicionarErro(ProdutoResource.ErroQuantidadeLote);

        if (codigoBase.Length > 0 && command.Quantidade > 0)
        {
            var maiorCodigo = $"{codigoBase}{command.Quantidade}";
            if (maiorCodigo.Length > 25)
                notificacoes.AdicionarErro(ProdutoResource.ErroCodigoGeradoMuitoLongo);

            if ($"{codigoBase}1".Length < 3)
                notificacoes.AdicionarErro(ProdutoResource.ErroCodigoLimiteMinimoDeCaractere);
        }

        return notificacoes.Messages;
    }
}
