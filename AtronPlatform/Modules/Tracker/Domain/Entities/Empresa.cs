#nullable enable

using System.Collections.Generic;
using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Entities
{
    public sealed class Empresa : EntityBase
    {
        public string Codigo { get; set; } = string.Empty;
        public string NomeFantasia { get; set; } = string.Empty;
        public Endereco Endereco { get; set; } = new();
        public string Numero { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public StatusEmpresa Status { get; internal set; } = StatusEmpresa.Pendente;
        public ICollection<UsuarioEmpresa> Usuarios { get; private set; } = new List<UsuarioEmpresa>();
    }
}
