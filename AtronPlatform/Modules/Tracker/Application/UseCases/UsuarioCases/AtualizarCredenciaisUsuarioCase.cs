using Domain.Entities;
using Domain.Interfaces.Identity;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Threading.Tasks;

namespace Application.UseCases.UsuarioCases
{
    public sealed class AtualizarCredenciaisUsuarioCase(IUsuarioIdentityRepository usuarioIdentityRepository)
    {
        private readonly IUsuarioIdentityRepository _usuarioIdentityRepository = usuarioIdentityRepository;

        public async Task<Resultado> ExecutarAsync(Usuario usuario, string? senha)
        {
            if (senha.IsNullOrEmpty())
                return Resultado.Sucesso();

            var atualizado = await _usuarioIdentityRepository
                .AtualizarUserIdentityRepositoryAsync(usuario.Codigo, usuario.Email, senha);

            return atualizado
                ? Resultado.Sucesso()
                : Resultado.Falha(UsuarioResource.ErroInesperadoAtualizacao);
        }
    }
}
