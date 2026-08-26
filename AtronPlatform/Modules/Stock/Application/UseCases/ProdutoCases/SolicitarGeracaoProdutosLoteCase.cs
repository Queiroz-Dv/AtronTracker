using AtronStock.Application.DTO.Request;
using AtronStock.Application.DTO.Response;
using AtronStock.Application.Mapping;
using AtronStock.Application.Resources;
using AtronStock.Application.Validacoes;
using AtronStock.Domain.Entities;
using AtronStock.Domain.Interfaces;
using AtronStock.Domain.ValueObjects;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using Shared.Extensions;

namespace AtronStock.Application.UseCases.ProdutoCases;

public sealed class SolicitarGeracaoProdutosLoteCase(
    IProcessamentoProdutoLoteRepository repository,
    GeracaoProdutosLoteValidador validador,
    SelecionarCategoriasProdutoCase selecionarCategorias,
    IUserAccessor userAccessor)
{
    public async Task<Resultado<SolicitacaoGeracaoProdutosLoteResponse>> ExecutarAsync(GerarProdutosLoteRequest request)
    {
        var command = GeracaoProdutosLoteCommand.Criar(request);
        var mensagens = validador.Validar(command);
        if (mensagens.TemErros())
            return Resultado<SolicitacaoGeracaoProdutosLoteResponse>.Falhas(mensagens);

        var categorias = await selecionarCategorias.ExecutarAsync(command.CategoriaCodigos);
        if (categorias.TeveFalha)
            return Resultado<SolicitacaoGeracaoProdutosLoteResponse>.Falhas(categorias.Messages);

        var solicitanteCodigo = userAccessor.ObterCodigoUsuarioLogado();
        if (string.IsNullOrWhiteSpace(solicitanteCodigo))
            return Resultado<SolicitacaoGeracaoProdutosLoteResponse>.Falha(
                ProdutoResource.ErroSolicitanteNaoIdentificado);

        var processamento = CriarProcessamento(command, solicitanteCodigo);
        if (!await repository.AdicionarAsync(processamento))
            return Resultado<SolicitacaoGeracaoProdutosLoteResponse>.Falha(
                ProdutoResource.ErroInesperadoSolicitarLote);

        return Resultado<SolicitacaoGeracaoProdutosLoteResponse>.Sucesso(new(
            processamento.Id,
            processamento.Status));
    }

    private static ProcessamentoProdutoLote CriarProcessamento(
        GeracaoProdutosLoteCommand command,
        string solicitanteCodigo)
        => new(new SolicitacaoGeracaoProdutosLote(
            command.CodigoBase.NormalizarCodigo(),
            command.Quantidade,
            solicitanteCodigo.Trim().ToUpperInvariant(),
            command.Descricao.Trim(),
            string.IsNullOrWhiteSpace(command.DescricaoComplementar)
                ? null
                : command.DescricaoComplementar.Trim(),
            command.DataAquisicao,
            command.PrecoUnitario,
            command.CategoriaCodigos
                .Where(codigo => !string.IsNullOrWhiteSpace(codigo))
                .Select(codigo => codigo.Trim().ToUpperInvariant())
                .Distinct()
                .ToList()));
}
