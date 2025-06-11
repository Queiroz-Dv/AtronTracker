namespace Shared.DTO.API
{
    public class DadosDoUsuario
    {
        public DadosDoUsuario()
        {
            CodigosPerfis = new List<string>();
            ModulosCodigo = new List<string>();
        }

        public string NomeDoUsuario { get; set; } = "";
        public string CodigoDoUsuario { get; set; } = "";
        public string Email { get; set; } = "";
        public string? CodigoDoDepartamento { get; set; }
        public string? CodigoDoCargo { get; set; }

        public DadosDoToken DadosDoToken { get; set; }

        public DateTime ExpiracaoDeAcesso { get; set; }
        
        public ICollection<string> CodigosPerfis { get; set; }
        public ICollection<string> ModulosCodigo { get; set; }
    }


    public class DadosDoToken
    {
        public DateTime ExpiracaoDoToken { get; set; }

        public DateTime ExpiracaoDoRefreshToken { get; set; }
    }
}