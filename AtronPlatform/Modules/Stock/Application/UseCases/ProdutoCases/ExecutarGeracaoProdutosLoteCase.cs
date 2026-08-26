using AtronStock.Application.DTO.Request;
using AtronStock.Application.DTO.Response;
using AtronStock.Application.Resources;
using AtronStock.Application.Validacoes;
using AtronStock.Domain.Interfaces;
using Shared.Domain.ValueObjects;
using Shared.Extensions;

namespace AtronStock.Application.UseCases.ProdutoCases;

public sealed class ExecutarGeracaoProdutosLoteCase(
    ILoteProdutoRepository loteRepository,
    GeracaoProdutosLoteValidador validador,
    SelecionarCategoriasProdutoCase selecionarCategorias,
    CriarLoteParaPersistenciaCase criarLoteParaPersistenciaCase
    )
{
    public async Task<Resultado<GeracaoProdutosLoteResultado>> ExecutarAsync(
        GeracaoProdutosLoteCommand command)
    {
        var mensagens = validador.Validar(command).ToList();
        if (mensagens.Count > 0)
            return Resultado<GeracaoProdutosLoteResultado>.Falhas(mensagens);

        var categorias = await selecionarCategorias.ExecutarAsync(command.CategoriaCodigos);
        if (categorias.TeveFalha)
            return Resultado<GeracaoProdutosLoteResultado>.Falhas(categorias.Messages);

        var codigoBase = command.CodigoBase.NormalizarCodigo();
        var codigosProdutos = Enumerable.Range(1, command.Quantidade)
            .Select(sequencia => $"{codigoBase}{sequencia}")
            .ToArray();

        var existentes = await loteRepository.ObterCodigosProdutosExistentesAsync(codigosProdutos);
        if (existentes.Count > 0)
            return Resultado<GeracaoProdutosLoteResultado>.Falha(string.Format(
                ProdutoResource.ErroCodigosGeradosExistentes,
                string.Join(", ", existentes.Take(5)))); // Obtém os cinco primeiros

        var lote = await criarLoteParaPersistenciaCase.ExecutarAsync(codigoBase, command, categorias.Dados!);
        if (!await loteRepository.AdicionarAsync(lote))
            return Resultado<GeracaoProdutosLoteResultado>.Falha(
                ProdutoResource.ErroInesperadoGerarLote);

        return Resultado<GeracaoProdutosLoteResultado>.Sucesso(new(
            lote.Id,
            lote.Codigo,
            lote.Produtos.Count));
    }
}