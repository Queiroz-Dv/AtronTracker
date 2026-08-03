using System.Collections.Generic;

namespace Application.DTO
{
    public class PlanejamentoCustoRelatorioDepartamentoDTO
    {
        public string DepartamentoCodigo { get; set; }

        public string DepartamentoDescricao { get; set; }

        public bool PossuiPlanejamento { get; set; }

        public string PlanejamentoCodigo { get; set; }

        public string PlanejamentoDescricao { get; set; }

        public bool ApenasDepartamento { get; set; }

        public decimal? ValorMinimoDepartamento { get; set; }

        public decimal? ValorTetoDepartamento { get; set; }

        public decimal SomaMinimosCargos { get; set; }

        public decimal SomaTetosCargos { get; set; }

        public decimal? PercentualOcupacaoTeto { get; set; }

        public int QuantidadeCargosNaoDetalhados { get; set; }

        public List<PlanejamentoCustoRelatorioCargoDTO> CargosDetalhados { get; set; } = [];

        public List<string> CargosPendentes { get; set; } = [];

        public List<string> Informacoes { get; set; } = [];
    }
}
