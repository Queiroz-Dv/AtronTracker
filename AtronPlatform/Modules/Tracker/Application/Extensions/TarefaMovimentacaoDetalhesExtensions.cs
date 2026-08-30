using Application.Records.Tarefa;
using Application.Resources;
using Domain.Extensions;
using System;
using System.Collections.Generic;

namespace Application.Extensions
{
    public static class TarefaMovimentacaoDetalhesExtensions
    {
        public static string CriarDetalhesDaAtualizacao(
            this AtualizacaoMovimentacaoRecord parametros)
        {
            var detalhes = new List<string>();
            var anterior = parametros.TarefaAnterior;
            var atual = parametros.TarefaAtual;

            if (!string.Equals(anterior.Titulo, atual.Titulo, StringComparison.Ordinal))
                detalhes.Add(TarefaResource.Historico_DetalheTituloAtualizado);

            if (!string.Equals(anterior.Conteudo, atual.Conteudo, StringComparison.Ordinal))
                detalhes.Add(TarefaResource.Historico_DetalheConteudoAtualizado);

            if (anterior.DataInicial != atual.DataInicial || anterior.DataFinal != atual.DataFinal)
            {
                detalhes.Add(string.Format(
                    TarefaResource.Historico_DetalhePeriodoAlterado,
                    anterior.DataInicial,
                    anterior.DataFinal,
                    atual.DataInicial,
                    atual.DataFinal));
            }

            if (anterior.TarefaEstadoId != atual.TarefaEstadoId)
            {
                detalhes.Add(string.Format(
                    TarefaResource.Historico_DetalheEstadoAlterado,
                    anterior.ObterEstado(),
                    atual.ObterEstado()));
            }

            if (!string.Equals(
                anterior.UsuarioCodigo,
                atual.UsuarioCodigo,
                StringComparison.Ordinal))
            {
                detalhes.Add(string.Format(
                    TarefaResource.Historico_DetalheResponsavelAlterado,
                    string.IsNullOrWhiteSpace(anterior.UsuarioCodigo)
                        ? TarefaResource.Historico_ValorNaoInformado
                        : anterior.UsuarioCodigo,
                    string.IsNullOrWhiteSpace(atual.UsuarioCodigo)
                        ? TarefaResource.Historico_ValorNaoInformado
                        : atual.UsuarioCodigo));
            }

            if (anterior.DestinoInicial != atual.DestinoInicial ||
                !string.Equals(
                    anterior.DepartamentoCodigo,
                    atual.DepartamentoCodigo,
                    StringComparison.Ordinal) ||
                !string.Equals(
                    anterior.CargoCodigo,
                    atual.CargoCodigo,
                    StringComparison.Ordinal))
            {
                detalhes.Add(TarefaResource.Historico_DetalheEscopoAtualizado);
            }

            if (anterior.ExigeAprovacaoParaObter != atual.ExigeAprovacaoParaObter)
                detalhes.Add(TarefaResource.Historico_DetalheAprovacaoAtualizada);

            return detalhes.Count == 0
                ? TarefaResource.Historico_DetalheAtualizacao
                : string.Join(" ", detalhes);
        }
    }
}
