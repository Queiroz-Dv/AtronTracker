using System;
using Domain.Enums;

namespace Domain.Entities
{
    public class SolicitacaoObtencaoTarefa : EntityBase
    {
        public int TarefaId { get; set; }

        public int SolicitanteId { get; set; }

        public string SolicitanteCodigo { get; set; }

        public int AprovadorId { get; set; }

        public string AprovadorCodigo { get; set; }

        public StatusSolicitacaoObtencaoTarefa Status { get; set; }

        public DateTime DataSolicitacao { get; set; }

        public DateTime? DataDecisao { get; set; }

        public Tarefa Tarefa { get; set; }

        public Usuario Solicitante { get; set; }

        public Usuario Aprovador { get; set; }

        public static SolicitacaoObtencaoTarefa CriarPendente(Tarefa tarefa, Usuario solicitante, Usuario aprovador)
        {
            return new SolicitacaoObtencaoTarefa
            {
                TarefaId = tarefa.Id,
                SolicitanteId = solicitante.Id,
                SolicitanteCodigo = solicitante.Codigo,
                AprovadorId = aprovador.Id,
                AprovadorCodigo = aprovador.Codigo,
                Status = StatusSolicitacaoObtencaoTarefa.Pendente,
                DataSolicitacao = DateTime.Now
            };
        }
    }
}
