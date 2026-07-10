using System;

namespace Domain.Entities
{
    public class ConfirmacaoEmail : EntityBase
    {
        public string UsuarioCodigo { get; set; }
        public string IdentificadorHash { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime ExpiraEm { get; set; }
        public DateTime? ConfirmadoEm { get; set; }
    }
}
