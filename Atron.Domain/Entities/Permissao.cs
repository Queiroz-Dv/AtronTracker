using System;

namespace Atron.Domain.Entities
{
    public class Permissao : EntityBase
    {       
        public int UsuarioId { get; set; }
        public string UsuarioCodigo { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public int PermissaoEstadoId { get; set; }
        public string Descricao { get; set; }
        public int QuantidadeDeDias { get; set; }
    }
}