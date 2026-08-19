using Domain.Enums;
using System;

namespace Application.DTO
{
    public class TarefaMovimentacaoDTO
    {
        public int Id { get; set; }

        public int TarefaId { get; set; }

        public string Movimento { get; set; }

        public string Detalhes { get; set; }

        public TipoMovimentacaoTarefa TipoMovimentacaoTarefa { get; set; }

        public string ResponsavelCodigo { get; set; }

        public string ResponsavelNome { get; set; }

        public DateTime DataOcorrencia { get; set; }
    }
}
