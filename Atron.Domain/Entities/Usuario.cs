using System;
using System.Collections.Generic;

namespace Atron.Domain.Entities
{
    public class Usuario : EntityBase
    {
        public string Codigo { get; set; }
        public string Nome { get; set; }
        public string Sobrenome { get; set; }
        public int? SalarioAtual { get; set; }
        public string Email { get; set; }

        public List<Tarefa> Tarefas { get; set; }
        public Salario Salario { get; set; }

        public DateTime? DataNascimento { get; set; }

        public ICollection<UsuarioCargoDepartamento> UsuarioCargoDepartamentos { get; set; }

        public ICollection<PerfilDeAcesso> PerfisDeAcesso { get; set; }

        public ICollection<PerfilDeAcessoUsuario> PerfilDeAcessoUsuarios { get; set; }
    }
}