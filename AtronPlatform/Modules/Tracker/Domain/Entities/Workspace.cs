using System.Collections.Generic;

namespace Domain.Entities
{
    public sealed class Workspace : EntityBase
    {
        public string Codigo { get; set; }
        public string Descricao { get; set; }

        public int? ResponsavelId { get; set; }
        public string ResponsavelCodigo { get; set; }
        public string ResponsavelEmail { get; set; }

        public Usuario Responsavel { get; set; }
        public ICollection<MembroWorkspace> Membros { get; set; } = new List<MembroWorkspace>();
    }
}
