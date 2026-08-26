using Application.DTO;
using Application.Resources;
using Domain.Entities;

namespace Application.Records.Tarefa
{
    public sealed record ContextoNotificacaoTarefaRecord(int TarefaId)
    {
        public string TarefaIdTexto => TarefaId > 0
            ? TarefaId.ToString()
            : TarefaResource.Descricao_TarefaNaoIdentificada;

        public string UrlEdicao => $"/atron/tarefas/editar/{TarefaId}";

        public string UrlSolicitacoes => "/atron/tarefas?visao=solicitacoes";

        public string ReferenciaExterna => $"tarefa:{TarefaId}";

        public string CriarChaveIdempotencia(
            string tipoEvento,
            string destinatarioCodigo)
        {
            return $"tracker:tarefa:{TarefaId}:{tipoEvento}:{destinatarioCodigo}";
        }

        public string CriarCorrelacaoId(string tipoEvento)
        {
            return $"tracker:{tipoEvento}:tarefa:{TarefaId}";
        }

        public static ContextoNotificacaoTarefaRecord Criar(TarefaDTO tarefa)
        {
            return new ContextoNotificacaoTarefaRecord(tarefa.Id);
        }

        public static ContextoNotificacaoTarefaRecord Criar(Domain.Entities.Tarefa tarefa)
        {
            return new ContextoNotificacaoTarefaRecord(tarefa.Id);
        }

        public static ContextoNotificacaoTarefaRecord Criar(
            SolicitacaoObtencaoTarefa solicitacao)
        {
            return new ContextoNotificacaoTarefaRecord(solicitacao.TarefaId);
        }
    }
}
