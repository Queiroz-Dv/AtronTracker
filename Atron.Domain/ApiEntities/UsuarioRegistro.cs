namespace Atron.Domain.ApiEntities
{
    public class UsuarioRegistro
    {
        public UsuarioRegistro() { }

        public UsuarioRegistro(string codigoDeAcesso, string email, string senha, string confirmarSenha)
        {
            CodigoDeAcesso = codigoDeAcesso;
            Email = email;
            Senha = senha;
            ConfirmarSenha = confirmarSenha;
        }

        public string CodigoDeAcesso { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public string ConfirmarSenha { get; set; }
    }
}
