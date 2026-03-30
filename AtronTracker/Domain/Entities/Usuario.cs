using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Usuario : EntityBase
    {
        public Usuario()
        {
            
        }

        public Usuario(string codigo, string nome, string sobrenome, string email, DateTime? dataNascimento)
        {
            Codigo = codigo;
            Nome = nome;
            Sobrenome = sobrenome;
            Email = email;
            DataNascimento = dataNascimento;
            Inativo = false;
        }

        public string Codigo { get; set; }
        public string Nome { get; set; }
        public string Sobrenome { get; set; }
        public string Email { get; set; }
        public DateTime? DataNascimento { get; set; }
        public int? SalarioAtual { get; set; }
        public bool Inativo { get; set; }
        public string CodigoReativacao { get; set; }

        public Salario Salario { get; set; }
        public ICollection<Tarefa> Tarefas { get; set; }
        public ICollection<UsuarioCargoDepartamento> UsuarioCargoDepartamentos { get; set; }
        public ICollection<PerfilDeAcessoUsuario> PerfisDeAcessoUsuario { get; set; }
    }
}