using Domain.Entities;

namespace Domain.Extensions
{
    public static class UsuarioObtencaoExtensions
    {
        public static string ObterNome(this Usuario usuario)
        {
            if (usuario != null)
            {
                var nome = $"{usuario.Nome} {usuario.Sobrenome}";
                return string.IsNullOrWhiteSpace(nome) ? usuario.Codigo : nome;
            }

            return string.Empty;
        }
    }
}