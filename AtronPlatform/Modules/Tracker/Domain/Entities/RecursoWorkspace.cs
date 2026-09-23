namespace Domain.Entities
{
    public class RecursoWorkspace
    {
        public int Id { get; set; }

        // Identificação do Tenant / Workspace
        public int WorkspaceId { get; set; }
        public string WorkspaceCodigo { get; set; }

        public Workspace Workspace { get; set; }

        // Identificação do Módulo
        public string ModuloCodigo { get; set; }

        // Registro
        public int RecursoId { get; set; }
        public string? RecursoCodigo { get; set; }
    }
}