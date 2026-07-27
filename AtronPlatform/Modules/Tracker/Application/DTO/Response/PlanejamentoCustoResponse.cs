namespace Application.DTO.Response
{
    using System.Collections.Generic;

    public class PlanejamentoCustoResponse
    {
        public int Id { get; set; }

        public string Codigo { get; set; }

        public string Descricao { get; set; }

        public int Ano { get; set; }

        public decimal ValorMinimo { get; set; }

        public decimal ValorTeto { get; set; }

        public bool ApenasDepartamento { get; set; }

        public string DepartamentoCodigo { get; set; }

        public string DepartamentoDescricao { get; set; }

        public List<PlanejamentoCustoCargoResponse> DetalhesCargo { get; set; } = [];
    }
}
