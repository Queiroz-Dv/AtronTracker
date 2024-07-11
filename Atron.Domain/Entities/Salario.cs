using System;

namespace Atron.Domain.Entities
{
    public class Salario : EntityBase
    {      
        public int UsuarioId { get; set; }

        public string UsuarioCodigo { get; set; }

        public int MesId { get; set; }

        public int SalarioMensal { get; set; }

        public DateTime? Ano { get; set; }
    }
}