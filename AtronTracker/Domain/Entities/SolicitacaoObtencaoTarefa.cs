using System;

namespace Domain.Entities
{
    public class SolicitacaoObtencaoTarefa : EntityBase
    {
        public int TarefaId { get; set; }

        public int SolicitanteId { get; set; }

        public string SolicitanteCodigo { get; set; }

        public int AprovadorId { get; set; }

        public string AprovadorCodigo { get; set; }

        public int Status { get; set; }

        public DateTime DataSolicitacao { get; set; }

        public DateTime? DataDecisao { get; set; }

        public Tarefa Tarefa { get; set; }

        public Usuario Solicitante { get; set; }

        public Usuario Aprovador { get; set; }
    }
}
