namespace Application.DTO
{
    public class PlanejamentoCustoRelatorioCargoDTO
    {
        public string CargoCodigo { get; set; }

        public string CargoDescricao { get; set; }

        public decimal ValorMinimo { get; set; }

        public decimal ValorTeto { get; set; }

        public decimal PercentualOcupacaoTetoDepartamento { get; set; }
    }
}
