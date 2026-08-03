using Application.Interfaces.Services;
using Application.Resources;
using AtronNotificacoes.Contracts;
using Domain.Entities;
using Shared.Extensions;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Application.Services.EntitiesServices.Tarefas
{
    public class TarefaNotificacaoInternaService : ITarefaNotificacaoInternaService
    {
        private readonly INotificacoesInternasPublisher _publisher;

        public TarefaNotificacaoInternaService(INotificacoesInternasPublisher publisher)
        {
            _publisher = publisher;
        }

        public Task NotificarAtribuicaoAsync(Tarefa tarefa, Usuario usuario)
        {
            return tarefa is null || usuario is null
                ? Task.CompletedTask
                : CriarAsync(
                    usuario.Codigo,
                    TarefaResource.Titulo_TarefaAtribuida,
                    Formatar(TarefaResource.Mensagem_TarefaAtribuidaUsuario, ObterIdentificador(tarefa)),
                    "TarefaAtribuida",
                    tarefa.Id);
        }

        public Task NotificarObtencaoAsync(Tarefa tarefa, Usuario usuario)
        {
            return tarefa is null || usuario is null
                ? Task.CompletedTask
                : CriarAsync(
                    usuario.Codigo,
                    TarefaResource.Titulo_TarefaObtida,
                    Formatar(TarefaResource.Mensagem_TarefaObtida, ObterIdentificador(tarefa)),
                    "TarefaObtida",
                    tarefa.Id);
        }

        public Task NotificarSolicitacaoRecebidaAsync(SolicitacaoObtencaoTarefa solicitacao)
        {
            if (solicitacao is null)
                return Task.CompletedTask;

            return CriarAsync(
                solicitacao.AprovadorCodigo,
                TarefaResource.Titulo_SolicitacaoRecebida,
                Formatar(
                    TarefaResource.Mensagem_SolicitacaoRecebida,
                    ObterNome(solicitacao.Solicitante) ?? solicitacao.SolicitanteCodigo,
                    ObterIdentificador(solicitacao.Tarefa, solicitacao.TarefaId)),
                "SolicitacaoObtencaoRecebida",
                solicitacao.TarefaId,
                "/atron/tarefas?visao=solicitacoes");
        }

        public Task NotificarDecisaoSolicitacaoAsync(SolicitacaoObtencaoTarefa solicitacao, bool aprovada)
        {
            if (solicitacao is null)
                return Task.CompletedTask;

            var identificador = ObterIdentificador(solicitacao.Tarefa, solicitacao.TarefaId);
            return CriarAsync(
                solicitacao.SolicitanteCodigo,
                aprovada ? TarefaResource.Titulo_SolicitacaoAprovada : TarefaResource.Titulo_SolicitacaoRecusada,
                aprovada
                    ? Formatar(TarefaResource.Mensagem_NotificacaoSolicitacaoAprovada, identificador)
                    : Formatar(TarefaResource.Mensagem_NotificacaoSolicitacaoRecusada, identificador),
                aprovada ? "SolicitacaoObtencaoAprovada" : "SolicitacaoObtencaoRecusada",
                solicitacao.TarefaId);
        }

        private async Task CriarAsync(string usuarioCodigo, string titulo, string mensagem, string tipoEvento, int tarefaId, string urlDestino = null)
        {
            if (usuarioCodigo.IsNullOrEmpty())
                return;

            try
            {
                await _publisher.PublicarAsync(new PublicarNotificacaoInternaRequest(
                    usuarioCodigo,
                    "Tracker",
                    tipoEvento,
                    titulo,
                    mensagem,
                    urlDestino ?? $"/atron/tarefas/editar/{tarefaId}",
                    $"tarefa:{tarefaId}",
                    DateTimeOffset.UtcNow,
                    $"tracker:tarefa:{tarefaId}:{tipoEvento}:{usuarioCodigo}",
                    $"tracker:{tipoEvento}:tarefa:{tarefaId}"));
            }
            catch
            {
                // A publicação é consultiva e não pode interromper o fluxo principal da tarefa.
            }
        }

        private static string ObterIdentificador(Tarefa tarefa, int? tarefaId = null)
        {
            return tarefa?.Identificador?.ToString() ?? tarefaId?.ToString() ?? TarefaResource.Descricao_TarefaNaoIdentificada;
        }

        private static string Formatar(string formato, params object[] argumentos)
        {
            return string.Format(CultureInfo.GetCultureInfo("pt-BR"), formato, argumentos);
        }

        private static string ObterNome(Usuario usuario)
        {
            if (usuario is null)
                return null;

            var nome = $"{usuario.Nome} {usuario.Sobrenome}".Trim();
            return nome.IsNullOrEmpty() ? usuario.Codigo : nome;
        }
    }
}
