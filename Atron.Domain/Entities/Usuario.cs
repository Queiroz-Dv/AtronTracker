using System;
using System.Collections.Generic;

namespace Atron.Domain.Entities
{
    public class Usuario : EntityBase
    {
        public string Codigo { get; set; }
        public string Nome { get; set; }
        public string Sobrenome { get; set; }
        public int Salario { get; set; }

        // Mapeamento de Cargo
        public int CargoId { get; set; }
        public string CargoCodigo { get; set; }
        public Cargo Cargo { get; set; }

        // Mapeamento de Departamento
        public int DepartamentoId { get; set; }
        public string DepartamentoCodigo { get; set; }
        public Departamento Departamento { get; set; }

        public List<Tarefa> Tarefas { get; set; }

        public DateTime? DataNascimento { get; set; }
    }
}