using System.Collections.Generic;
using Domain.Enums;

namespace Domain.Entities
{
    public sealed class Workspace : EntityBase
    {
        public string Nome { get; set; } = string.Empty;
        public TipoWorkspace Tipo { get; set; }
        public int? EmpresaId { get; set; }

        public Empresa Empresa { get; set; }
        public ICollection<MembroWorkspace> Membros { get; set; } = new List<MembroWorkspace>();
    }
}
