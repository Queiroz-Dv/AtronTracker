using Shared.DTO.API;
using System;
using System.Collections.Generic;

namespace Atron.Application.DTO
{
    public class UsuarioDTO
    {
        public int Id { get; set; }

        public string Codigo { get; set; }

        public string Nome { get; set; }

        public string Sobrenome { get; set; }

        public DateTime? DataNascimento { get; set; }

        public int? Salario { get; set; }

        public string CargoCodigo { get; set; }

        public string DepartamentoCodigo { get; set; }

        public string PerfilDeAcessoCodigo { get; set; }

        public string Email { get; set; }

        public string Senha { get; set; }

        public DadosDeTokenComRefreshToken DadosDoToken { get; set; }

        public DepartamentoDTO Departamento { get; set; }

        public CargoDTO Cargo { get; set; }

        public List<PerfilDeAcessoDTO> PerfisDeAcesso { get; set; }
    }
}