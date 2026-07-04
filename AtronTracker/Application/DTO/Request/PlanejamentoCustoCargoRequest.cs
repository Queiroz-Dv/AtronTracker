namespace Application.DTO.Request
{
    public class PlanejamentoCustoCargoRequest
    {
        public string CargoCodigo { get; set; }

        public bool Detalhado { get; set; }

        public decimal? ValorMinimo { get; set; }

        public decimal? ValorTeto { get; set; }
    }
}
