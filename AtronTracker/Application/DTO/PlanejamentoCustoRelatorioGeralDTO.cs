using System.Collections.Generic;

namespace Application.DTO
{
    public class PlanejamentoCustoRelatorioGeralDTO
    {
        public int Ano { get; set; }

        public List<PlanejamentoCustoRelatorioDepartamentoDTO> Departamentos { get; set; } = [];
    }
}
