namespace Application.DTO.Response
{
    public class PlanejamentoCustoCargoResponse
    {
        public string CargoCodigo { get; set; }

        public string CargoDescricao { get; set; }

        public bool Detalhado { get; set; }

        public decimal? ValorMinimo { get; set; }

        public decimal? ValorTeto { get; set; }
    }
}
