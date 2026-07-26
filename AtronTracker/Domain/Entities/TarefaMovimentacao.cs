using Domain.Enums;
using System;

namespace Domain.Entities
{
    public class TarefaMovimentacao : EntityBase
    {
        public int TarefaId { get; set; }

        public TipoMovimentacaoTarefa Tipo { get; set; }

        public string Descricao { get; set; }

        public string ResponsavelCodigo { get; set; }

        public string ResponsavelNome { get; set; }

        public DateTime DataOcorrencia { get; set; }

        public Tarefa Tarefa { get; set; }
    }
}
