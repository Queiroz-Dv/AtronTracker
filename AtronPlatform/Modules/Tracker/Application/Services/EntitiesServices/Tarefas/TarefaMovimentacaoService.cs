using Application.DTO;
using Application.Interfaces.Services;
using Application.Resources;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Queries;
using Shared.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;

namespace Application.Services.EntitiesServices.Tarefas
{
    public class TarefaMovimentacaoService(
        ITarefaMovimentacaoRepository movimentacaoRepository,
        ITarefaRepository tarefaRepository,
        ITarefaUsuarioAtualService usuarioAtualService) : ITarefaMovimentacaoService
    {
        private const int TamanhoPaginaPadrao = 5;
        private const int TamanhoPaginaMaximo = 50;
        private readonly ITarefaMovimentacaoRepository _movimentacaoRepository = movimentacaoRepository;
        private readonly ITarefaRepository _tarefaRepository = tarefaRepository;
        private readonly ITarefaUsuarioAtualService _usuarioAtualService = usuarioAtualService;

        public Task<Resultado> RegistrarCriacaoAsync(Tarefa tarefa, Usuario responsavel)
        {
            var estado = ObterEstado(tarefa);
            return RegistrarAsync(new RegistroMovimentacao(
                tarefa.Id,
                TipoMovimentacaoTarefa.Criacao,
                Formatar("Historico_DetalheCriacao", estado),
                responsavel));
        }

        public Task<Resultado> RegistrarAtualizacaoAsync(
            Tarefa tarefaAnterior,
            Tarefa tarefaAtual,
            Usuario responsavel)
        {
            var detalhes = ObterDetalhesAtualizacao(tarefaAnterior, tarefaAtual);
            return RegistrarAsync(new RegistroMovimentacao(
                tarefaAnterior.Id,
                TipoMovimentacaoTarefa.Atualizacao,
                detalhes,
                responsavel));
        }

        public Task<Resultado> RegistrarObtencaoAsync(Tarefa tarefa, Usuario responsavel)
        {
            return RegistrarAsync(new RegistroMovimentacao(
                tarefa.Id,
                TipoMovimentacaoTarefa.Obtencao,
                Formatar("Historico_DetalheObtencao", ObterNome(responsavel)),
                responsavel));
        }

        public Task<Resultado> RegistrarSolicitacaoAsync(
            SolicitacaoObtencaoTarefa solicitacao,
            Usuario responsavel)
        {
            return RegistrarAsync(new RegistroMovimentacao(
                solicitacao.TarefaId,
                TipoMovimentacaoTarefa.SolicitacaoObtencao,
                Formatar("Historico_DetalheSolicitacao", ObterNome(solicitacao.Aprovador)),
                responsavel));
        }

        public Task<Resultado> RegistrarDecisaoAsync(
            SolicitacaoObtencaoTarefa solicitacao,
            Usuario responsavel,
            bool aprovar)
        {
            var tipo = aprovar
                ? TipoMovimentacaoTarefa.AprovacaoObtencao
                : TipoMovimentacaoTarefa.RecusaObtencao;
            var descricao = aprovar
                ? Formatar(
                    "Historico_DetalheAprovacao",
                    ObterNome(solicitacao.Solicitante),
                    ObterEstado(solicitacao.Tarefa))
                : Formatar(
                    "Historico_DetalheRecusa",
                    ObterNome(solicitacao.Solicitante));

            return RegistrarAsync(new RegistroMovimentacao(
                solicitacao.TarefaId,
                tipo,
                descricao,
                responsavel));
        }

        public async Task<Resultado<TarefaMovimentacaoPaginaDTO>> ObterAsync(
            int tarefaId,
            int pagina,
            int tamanhoPagina)
        {
            var usuario = await _usuarioAtualService.ObterAsync();
            if (usuario.TeveFalha)
                return Resultado<TarefaMovimentacaoPaginaDTO>.Falhas(usuario.Messages);

            if (!await _tarefaRepository.PodeAcessarHistoricoAsync(
                tarefaId,
                usuario.Dados.Id,
                usuario.Dados.Codigo))
            {
                return Resultado<TarefaMovimentacaoPaginaDTO>.Falha(
                    ObterResource("Erro_AcessoHistoricoNaoPermitido"));
            }

            var paginaNormalizada = Math.Max(pagina, 1);
            var tamanhoNormalizado = tamanhoPagina <= 0
                ? TamanhoPaginaPadrao
                : Math.Min(tamanhoPagina, TamanhoPaginaMaximo);
            var consulta = new TarefaMovimentacaoConsulta(
                tarefaId,
                paginaNormalizada,
                tamanhoNormalizado);
            var resultado = await _movimentacaoRepository.ObterPaginaAsync(consulta);

            return Resultado<TarefaMovimentacaoPaginaDTO>.Sucesso(new TarefaMovimentacaoPaginaDTO
            {
                Itens = resultado.Itens.Select(Mapear).ToList(),
                TotalItens = resultado.TotalItens,
                Pagina = paginaNormalizada,
                TamanhoPagina = tamanhoNormalizado
            });
        }

        private async Task<Resultado> RegistrarAsync(RegistroMovimentacao registro)
        {
            var movimentacao = new TarefaMovimentacao
            {
                TarefaId = registro.TarefaId,
                Tipo = registro.Tipo,
                Descricao = registro.Descricao,
                ResponsavelCodigo = registro.Responsavel.Codigo,
                ResponsavelNome = ObterNome(registro.Responsavel),
                DataOcorrencia = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
            };

            return await _movimentacaoRepository.RegistrarAsync(movimentacao)
                ? Resultado.Sucesso()
                : Resultado.Falha(ObterResource("Erro_RegistrarMovimentacao"));
        }

        private static TarefaMovimentacaoDTO Mapear(TarefaMovimentacao movimentacao)
        {
            return new TarefaMovimentacaoDTO
            {
                Id = movimentacao.Id,
                Movimento = ObterResource($"Historico_Movimento_{movimentacao.Tipo}"),
                Detalhes = movimentacao.Descricao,
                ResponsavelCodigo = movimentacao.ResponsavelCodigo,
                ResponsavelNome = movimentacao.ResponsavelNome,
                DataOcorrencia = DateTime.SpecifyKind(movimentacao.DataOcorrencia, DateTimeKind.Utc)
            };
        }

        private static string ObterDetalhesAtualizacao(Tarefa anterior, Tarefa atual)
        {
            var detalhes = new List<string>();

            if (!string.Equals(anterior.Titulo, atual.Titulo, StringComparison.Ordinal))
                detalhes.Add(ObterResource("Historico_DetalheTituloAtualizado"));

            if (!string.Equals(anterior.Conteudo, atual.Conteudo, StringComparison.Ordinal))
                detalhes.Add(ObterResource("Historico_DetalheConteudoAtualizado"));

            if (anterior.DataInicial != atual.DataInicial || anterior.DataFinal != atual.DataFinal)
            {
                detalhes.Add(Formatar(
                    "Historico_DetalhePeriodoAlterado",
                    anterior.DataInicial,
                    anterior.DataFinal,
                    atual.DataInicial,
                    atual.DataFinal));
            }

            if (anterior.TarefaEstadoId != atual.TarefaEstadoId)
            {
                detalhes.Add(Formatar(
                    "Historico_DetalheEstadoAlterado",
                    ObterEstado(anterior),
                    ObterEstado(atual)));
            }

            if (!string.Equals(anterior.UsuarioCodigo, atual.UsuarioCodigo, StringComparison.OrdinalIgnoreCase))
            {
                detalhes.Add(Formatar(
                    "Historico_DetalheResponsavelAlterado",
                    ObterCodigoOuNaoInformado(anterior.UsuarioCodigo),
                    ObterCodigoOuNaoInformado(atual.UsuarioCodigo)));
            }

            if (anterior.DestinoInicial != atual.DestinoInicial ||
                !string.Equals(anterior.DepartamentoCodigo, atual.DepartamentoCodigo, StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(anterior.CargoCodigo, atual.CargoCodigo, StringComparison.OrdinalIgnoreCase))
            {
                detalhes.Add(ObterResource("Historico_DetalheEscopoAtualizado"));
            }

            if (anterior.ExigeAprovacaoParaObter != atual.ExigeAprovacaoParaObter)
                detalhes.Add(ObterResource("Historico_DetalheAprovacaoAtualizada"));

            return detalhes.Count == 0
                ? ObterResource("Historico_DetalheAtualizacao")
                : string.Join(" ", detalhes);
        }

        private static string ObterEstado(Tarefa tarefa)
        {
            return tarefa.EstadoDaTarefa?.Descricao ??
                   tarefa.TarefaEstadoId.ToString(CultureInfo.InvariantCulture);
        }

        private static string ObterCodigoOuNaoInformado(string codigo)
        {
            return string.IsNullOrWhiteSpace(codigo)
                ? ObterResource("Historico_ValorNaoInformado")
                : codigo;
        }

        private static string ObterNome(Usuario usuario)
        {
            if (usuario is null)
                return ObterResource("Historico_ValorNaoInformado");

            var nome = $"{usuario.Nome} {usuario.Sobrenome}".Trim();
            return string.IsNullOrWhiteSpace(nome) ? usuario.Codigo : nome;
        }

        private static string Formatar(string chave, params object[] argumentos)
            => string.Format(CultureInfo.GetCultureInfo("pt-BR"), ObterResource(chave), argumentos);

        private static string ObterResource(string chave)
            => TarefaResource.ResourceManager.GetString(chave, TarefaResource.Culture)
               ?? throw new MissingManifestResourceException(chave);

        private sealed record RegistroMovimentacao(
            int TarefaId,
            TipoMovimentacaoTarefa Tipo,
            string Descricao,
            Usuario Responsavel);
    }
}
