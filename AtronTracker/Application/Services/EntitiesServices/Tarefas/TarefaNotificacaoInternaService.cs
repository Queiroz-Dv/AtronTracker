using Application.Interfaces.Services;
using Application.Resources;
using Domain.Entities;
using Shared.Extensions;
using System.Globalization;
using System.Threading.Tasks;

namespace Application.Services.EntitiesServices.Tarefas
{
    public class TarefaNotificacaoInternaService : ITarefaNotificacaoInternaService
    {
        private readonly INotificacaoInternaService _notificacaoInternaService;

        public TarefaNotificacaoInternaService(INotificacaoInternaService notificacaoInternaService)
        {
            _notificacaoInternaService = notificacaoInternaService;
        }

        public Task NotificarAtribuicaoAsync(Tarefa tarefa, Usuario usuario)
        {
            return tarefa is null || usuario is null
                ? Task.CompletedTask
                : CriarAsync(
                    usuario.Id,
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
                    usuario.Id,
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
                solicitacao.AprovadorId,
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
                solicitacao.SolicitanteId,
                solicitacao.SolicitanteCodigo,
                aprovada ? TarefaResource.Titulo_SolicitacaoAprovada : TarefaResource.Titulo_SolicitacaoRecusada,
                aprovada
                    ? Formatar(TarefaResource.Mensagem_NotificacaoSolicitacaoAprovada, identificador)
                    : Formatar(TarefaResource.Mensagem_NotificacaoSolicitacaoRecusada, identificador),
                aprovada ? "SolicitacaoObtencaoAprovada" : "SolicitacaoObtencaoRecusada",
                solicitacao.TarefaId);
        }

        private async Task CriarAsync(int usuarioId, string usuarioCodigo, string titulo, string mensagem, string tipoEvento, int tarefaId, string urlDestino = null)
        {
            if (usuarioCodigo.IsNullOrEmpty())
                return;

            await _notificacaoInternaService.CriarAsync(new NotificacaoInterna
            {
                UsuarioId = usuarioId,
                UsuarioCodigo = usuarioCodigo,
                Titulo = titulo,
                Mensagem = mensagem,
                Modulo = TarefaResource.Descricao_ModuloTarefas,
                TipoEvento = tipoEvento,
                TarefaId = tarefaId,
                UrlDestino = urlDestino ?? $"/atron/tarefas/editar/{tarefaId}",
                Lida = false
            });
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
