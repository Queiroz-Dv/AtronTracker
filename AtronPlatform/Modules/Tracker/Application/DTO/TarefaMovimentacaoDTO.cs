using System;
using System.Collections.Generic;

namespace Application.DTO
{
    public class TarefaMovimentacaoDTO
    {
        public int Id { get; set; }

        public string Movimento { get; set; }

        public string Detalhes { get; set; }

        public string ResponsavelCodigo { get; set; }

        public string ResponsavelNome { get; set; }

        public DateTime DataOcorrencia { get; set; }
    }

    public class TarefaMovimentacaoPaginaDTO
    {
        public IReadOnlyCollection<TarefaMovimentacaoDTO> Itens { get; set; }

        public int TotalItens { get; set; }

        public int Pagina { get; set; }

        public int TamanhoPagina { get; set; }
    }
}
