namespace Application.DTO
{
    public class PlanejamentoCustoCargoDTO
    {
        public int Id { get; set; }

        public int CargoId { get; set; }

        public string CargoCodigo { get; set; }

        public string CargoDescricao { get; set; }

        public bool Detalhado { get; set; }

        public decimal? ValorMinimo { get; set; }

        public decimal? ValorTeto { get; set; }
    }
}
