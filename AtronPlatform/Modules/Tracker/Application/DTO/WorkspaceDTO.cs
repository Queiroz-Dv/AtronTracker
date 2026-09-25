namespace Application.DTO
{
    public class WorkspaceDTO
    {
        public string Codigo { get; set; }
        public string Descricao { get; set; }

        public UsuarioDTO Responsavel { get; set; }
    }
}