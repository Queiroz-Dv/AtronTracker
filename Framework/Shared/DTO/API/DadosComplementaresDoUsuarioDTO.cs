namespace Shared.DTO.API
{
    public class DadosComplementaresDoUsuarioDTO
    {
        public DadosDoUsuarioDTO DadosDoUsuario { get; init; } = new();

        public List<DadosDoPerfilDTO> DadosDoPerfil { get; init; } = new();

        public TempoDosTokensDoUsuarioDTO DadosDoToken { get; init; }
    }

    public class DadosDoUsuarioDTO
    {
        public string CodigoDoUsuario { get; set; } = string.Empty;

        public string NomeDoUsuario { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string CodigoDoDepartamento { get; set; } = string.Empty;

        public string CodigoDoCargo { get; set; } = string.Empty;
    }

    public class DadosDoPerfilDTO
    {    
        public string CodigoPerfil { get; set; } = string.Empty;

        public List<DadosDoModuloDTO> Modulos { get; set; } = new();
    }

    public class DadosDoModuloDTO
    {
        public DadosDoModuloDTO() { }

        public DadosDoModuloDTO(string codigo, string descricao)
        {
            Codigo = codigo;
            Descricao = descricao;
        }

        public string Codigo { get; set; }
        public string Descricao { get; set; }
    }

    public record TempoDosTokensDoUsuarioDTO(DateTime ExpiracaoDoToken, DateTime ExpiracaoDoRefreshToken);
}