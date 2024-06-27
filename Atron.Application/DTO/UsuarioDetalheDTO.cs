using System;

namespace Atron.Application.DTO
{
    public class UsuarioDetalheDTO
    {
        public string Codigo { get; set; }
        public string Nome { get; set; }
        public string Sobrenome { get; set; }
        public DateTime? DataNascimento { get; set; }
        public int Salario { get; set; }

        public string CargoCodigo { get; set; }
        public string CargoDescricao { get; set; }

        public string DepartamentoCodigo { get; set; }
        public string DepartamentoDescricao { get; set; }

    }
}