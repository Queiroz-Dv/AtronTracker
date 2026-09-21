using Domain.Entities;

namespace Domain.Tenants
{
    public class DepartamentoWorkspace
    {
        public int DepartamentoId { get; set; }
        public string DepartamentoCodigo { get; set; }

        public int WorkspaceId { get; set; }
        public string WorkspaceCodigo { get; set; }

        public Departamento Departamento { get; set; }
        public Workspace Workspace { get; set; }
    }
}