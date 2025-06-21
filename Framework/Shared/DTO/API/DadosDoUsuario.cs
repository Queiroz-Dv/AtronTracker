namespace Shared.DTO.API
{
    public class DadosDoUsuario        
    {    
        public string NomeDoUsuario { get; set; } = string.Empty;

        public string CodigoDoUsuario { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string CodigoDoDepartamento { get; set; } = string.Empty;

        public string CodigoDoCargo { get; set; } = string.Empty;

        public List<PerfilComModulos> PerfisDeAcesso { get; set; } = new();

        public DadosDoToken DadosDoToken { get; init; }
    }
    
    public class PerfilComModulos
    {
        public string CodigoPerfil { get; set; } = string.Empty;

        public List<DadosDoModulo> Modulos { get; set; } = new();
    }

    public record DadosDoModulo(string Codigo, string Descricao);

    public record DadosDoToken(DateTime ExpiracaoDoToken, DateTime ExpiracaoDoRefreshToken);

}