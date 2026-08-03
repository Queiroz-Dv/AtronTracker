namespace Application.DTO.Response
{
    public class PlanejamentoCustoRelatorioCargoResponse
    {
        public string CargoCodigo { get; set; }

        public string CargoDescricao { get; set; }

        public decimal ValorMinimo { get; set; }

        public decimal ValorTeto { get; set; }

        public decimal PercentualOcupacaoTetoDepartamento { get; set; }
    }
}
