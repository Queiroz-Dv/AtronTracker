namespace Application.DTO.Request
{
    public class UsuarioPerfilRequest
    {
        public string Codigo { get; set; }
    }

    public class PerfilUsuarioRequest
    {
        public string CodigoPerfil { get; set; }

        public UsuarioPerfilRequest[] Usuarios { get; set; } = [];
    }
}
