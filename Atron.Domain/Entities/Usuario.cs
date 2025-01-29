using System;
using System.Collections.Generic;

namespace Atron.Domain.Entities
{
    public class Usuario : EntityBase
    {
        public Usuario() { }

        public Usuario(string codigo, string nome = "", string sobrenome = "", string email = "")
        {
            Codigo = codigo;
            Nome = nome;
            Sobrenome = sobrenome;
            Email = email;
        }

        public string Codigo { get; set; }
        public string Nome { get; set; }
        public string Sobrenome { get; set; }
        public int? SalarioAtual { get; set; }
        public string Email { get; set; }

        public List<Tarefa> Tarefas { get; set; }
        public Salario Salario { get; set; }

        public DateTime? DataNascimento { get; set; }

        public List<UsuarioCargoDepartamento> UsuarioCargoDepartamentos { get; set; }
    }
}