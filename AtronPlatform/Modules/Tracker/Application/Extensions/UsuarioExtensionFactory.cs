using Application.DTO;

namespace Application.Extensions
{
    public static class UsuarioExtensionFactory
    {
        public static string ObterNome(this UsuarioDTO usuario)
        {
            var nome = $"{usuario.Nome} {usuario.Sobrenome}";
            return string.IsNullOrWhiteSpace(nome) ? usuario.Codigo : nome;
        }
    }
}