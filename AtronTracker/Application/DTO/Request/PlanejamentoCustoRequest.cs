namespace Application.DTO.Request
{
    using System.Collections.Generic;

    public class PlanejamentoCustoRequest
    {
        public string Codigo { get; set; }

        public string Descricao { get; set; }

        public int Ano { get; set; }

        public decimal ValorMinimo { get; set; }

        public decimal ValorTeto { get; set; }

        public bool ApenasDepartamento { get; set; }

        public string DepartamentoCodigo { get; set; }

        public List<PlanejamentoCustoCargoRequest> DetalhesCargo { get; set; } = [];
    }
}
