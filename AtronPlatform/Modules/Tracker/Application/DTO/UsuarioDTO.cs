using Shared.Application.DTOS.Auth;
using System;
using System.Collections.Generic;

namespace Application.DTO
{
    public class UsuarioDTO
    {
        public int Id { get; set; }

        public string Codigo { get; set; }

        public string Nome { get; set; }

        public string Sobrenome { get; set; }

        public DateTime? DataNascimento { get; set; }

        public string CargoCodigo { get; set; }

        public string DepartamentoCodigo { get; set; }

        public string PerfilDeAcessoCodigo { get; set; }

        public string Email { get; set; }

        public bool EmailConfirmado { get; set; }

        public string Senha { get; set; }

        public bool ReceberNotificacaoInternaTarefa { get; set; }
        public bool ReceberNotificacaoTarefaPorEmail { get; set; }
        public string GestorImediatoCodigo { get; set; }
        public string GestorImediatoNome { get; set; }

        public DadosDeTokenComRefreshToken DadosDoToken { get; set; }

        public DepartamentoDTO Departamento { get; set; }

        public CargoDTO Cargo { get; set; }

        public List<PerfilDeAcessoDTO> PerfisDeAcesso { get; set; }
    }
}
