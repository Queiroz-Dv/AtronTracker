using System;
using System.Collections.Generic;

namespace Atron.Application.DTO.Response
{
    public class UsuarioResponse
    {
        public string Codigo { get; set; }
        public string Nome { get; set; }
        public string Sobrenome { get; set; }
        public DateTime? DataNascimento { get; set; }
        public string Email { get; set; }
        public int? Salario { get; set; }

        // Cargo
        public string CargoCodigo { get; set; }
        public string? CargoDescricao { get; set; }

        // Departamento
        public string DepartamentoCodigo { get; set; }
        public string? DepartamentoDescricao { get; set; }

        public List<PerfilDeAcessoDTO> PerfisDeAcesso { get; set; }
    }
}
