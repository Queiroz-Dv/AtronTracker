namespace Application.DTO
{
    using System.Collections.Generic;

    public class PlanejamentoCustoDTO
    {
        public int Id { get; set; }

        public string Codigo { get; set; }

        public string Descricao { get; set; }

        public int Ano { get; set; }

        public decimal ValorMinimo { get; set; }

        public decimal ValorTeto { get; set; }

        public bool ApenasDepartamento { get; set; }

        public int DepartamentoId { get; set; }

        public string DepartamentoCodigo { get; set; }

        public string DepartamentoDescricao { get; set; }

        public DepartamentoDTO Departamento { get; set; }

        public List<PlanejamentoCustoCargoDTO> DetalhesCargo { get; set; } = [];
    }
}
