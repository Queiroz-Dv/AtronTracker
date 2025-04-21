namespace Shared.DTO.API
{
    public class DadosDoUsuario
    {
        public string NomeDoUsuario { get; set; } = "";
        public string CodigoDoUsuario { get; set; } = "";
        public string Email { get; set; } = "";
        public string? CodigoDoDepartamento { get; set; }
        public string? CodigoDoCargo { get; set; }
        public DateTime Expiracao { get; set; }
    }
}