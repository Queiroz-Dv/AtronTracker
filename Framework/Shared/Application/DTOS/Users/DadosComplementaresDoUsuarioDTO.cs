namespace Shared.Application.DTOS.Users
{
    public class DadosComplementaresDoUsuarioDTO
    {
        public DadosDoUsuarioDTO DadosDoUsuario { get; init; } = new();

        public List<DadosDoPerfilDTO> DadosDoPerfil { get; init; } = new();

        public DadosDaEmpresaDTO? DadosDaEmpresa { get; init; }

        public TempoDosTokensDoUsuarioDTO DadosDoToken { get; init; }
    }

    public sealed class DadosDaEmpresaDTO
    {
        public int Id { get; init; }
        public string Codigo { get; init; } = string.Empty;
        public string NomeFantasia { get; init; } = string.Empty;
        public bool AcessoPermitido { get; init; }
    }

    public class DadosDoUsuarioDTO
    {
        public string CodigoDoUsuario { get; set; } = string.Empty;

        public string NomeDoUsuario { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string CodigoDoDepartamento { get; set; } = string.Empty;

        public string CodigoDoCargo { get; set; } = string.Empty;
    }

    public class DadosDoPerfilDTO(string codigoPerfil = null)
    {
        public string CodigoPerfil { get; set; } = codigoPerfil;

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
