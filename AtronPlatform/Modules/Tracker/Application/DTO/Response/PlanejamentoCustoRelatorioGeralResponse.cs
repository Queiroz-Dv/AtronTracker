using System.Collections.Generic;

namespace Application.DTO.Response
{
    public class PlanejamentoCustoRelatorioGeralResponse
    {
        public int Ano { get; set; }

        public List<PlanejamentoCustoRelatorioDepartamentoResponse> Departamentos { get; set; } = [];
    }
}
