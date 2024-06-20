using System;

namespace Atron.Domain.Entities
{
    public class Permissao
    {
        public int Id { get; set; }
        public int Usuario { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public int PermissaoEstado { get; set; }
        public string Descricao { get; set; }
        public int PermissaoDia { get; set; }
    }
}