using AtronStock.Application.DTO.Request;
using AtronStock.Application.Mapping;
using AtronStock.Domain.Entities;
using AtronStock.Domain.Interfaces;

namespace AtronStock.Application.UseCases.ProdutoCases
{
    public sealed class CriarLoteParaPersistenciaCase(
        ILoteProdutoRepository loteRepository,
        ProdutoMapping mapper,
        TimeProvider timeProvider)
    {
        public async Task<LoteProduto> ExecutarAsync(string codigoBase, GeracaoProdutosLoteCommand command, IReadOnlyCollection<Categoria> categorias)
        {
            var prefixo = $"{codigoBase}_{timeProvider.GetLocalNow().Year}";
            var codigosExistentes = await loteRepository.ObterCodigosPorPrefixoAsync(prefixo);
            var codigoLote = ObterProximoCodigoLote(prefixo, codigosExistentes);
            var lote = new LoteProduto { Codigo = codigoLote };

            for (var sequencia = 1; sequencia <= command.Quantidade; sequencia++)
            {
                var request = new ProdutoRequest
                {
                    Codigo = $"{codigoBase}{sequencia}",
                    Descricao = command.Descricao,
                    DescricaoComplementar = command.DescricaoComplementar,
                    DataAquisicao = command.DataAquisicao,
                    PrecoUnitario = command.PrecoUnitario
                };
                var produto = mapper.MapToEntity(new(request, categorias));
                produto.LoteProduto = lote;
                lote.Produtos.Add(produto);
            }

            return lote;
        }

        private static string ObterProximoCodigoLote(string prefixo, IReadOnlyCollection<string> codigosExistentes)
        {
            var codigos = codigosExistentes.ToHashSet(StringComparer.OrdinalIgnoreCase);
            if (!codigos.Contains(prefixo))
                return prefixo;

            for (var sufixo = 2; ; sufixo++)
            {
                var candidato = $"{prefixo}_{sufixo}";
                if (!codigos.Contains(candidato))
                    return candidato;
            }
        }
    }
}
