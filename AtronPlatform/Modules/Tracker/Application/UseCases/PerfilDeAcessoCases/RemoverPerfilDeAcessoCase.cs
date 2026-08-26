using Application.Interfaces.Services;
using Application.Resources;
using Domain.Interfaces;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.UseCases.PerfilDeAcessoCases
{
    public sealed class RemoverPerfilDeAcessoCase(
        IPerfilDeAcessoRepository perfilDeAcessoRepository,
        IPerfilDeAcessoCacheInvalidator cacheInvalidator)
    {
        private readonly IPerfilDeAcessoRepository _perfilDeAcessoRepository = perfilDeAcessoRepository;
        private readonly IPerfilDeAcessoCacheInvalidator _cacheInvalidator = cacheInvalidator;

        public async Task<Resultado> ExecutarAsync(string codigo)
        {
            var perfil = await _perfilDeAcessoRepository.ObterPerfilPorCodigoRepositoryAsync(codigo);
            if (perfil is null)
                return Resultado.Falha(PerfilDeAcessoResource.Erro_RegistroNaoEncontrado);

            var removido = await _perfilDeAcessoRepository.DeletarPerfilRepositoryAsync(perfil);
            if (!removido)
                return Resultado.Falha(PerfilDeAcessoResource.Erro_RemoverPerfil);

            _cacheInvalidator.InvalidarUsuariosDoPerfil(perfil);
            return Resultado.Sucesso(PerfilDeAcessoResource.Mensagem_PerfilRemovido);
        }
    }
}
