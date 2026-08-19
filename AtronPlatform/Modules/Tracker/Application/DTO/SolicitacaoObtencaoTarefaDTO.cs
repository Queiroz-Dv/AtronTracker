using System;

namespace Application.DTO
{
    public class SolicitacaoObtencaoTarefaDTO
    {
        public int Id { get; set; }

        public int TarefaId { get; set; }

        public int Status { get; set; }

        public bool Aprovar { get; set; }

        public DateTime DataSolicitacao { get; set; }

        public DateTime? DataDecisao { get; set; }

        public TarefaDTO Tarefa { get; set; }

        public UsuarioDTO Solicitante { get; set; }

        public UsuarioDTO Aprovador { get; set; }
    }
}