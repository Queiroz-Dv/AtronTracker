using Application.DTO;
using Application.Interfaces.Mapping;
using Application.Resources;
using Domain.Interfaces;
using Shared.Domain.ValueObjects;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.UseCases.PerfilDeAcessoCases
{
    public sealed class ObterPerfilDeAcessoCase(
        IPerfilDeAcessoMapping map,
        IPerfilDeAcessoRepository perfilDeAcessoRepository)
    {
        private readonly IPerfilDeAcessoMapping _map = map;
        private readonly IPerfilDeAcessoRepository _perfilDeAcessoRepository = perfilDeAcessoRepository;

        public async Task<Resultado<List<PerfilDeAcessoDTO>>> ObterTodosAsync()
        {
            var perfis = await _perfilDeAcessoRepository.ObterTodosPerfisRepositoryAsync();
            return Resultado<List<PerfilDeAcessoDTO>>.Sucesso(_map.MapToDtos(perfis).ToList());
        }

        public async Task<Resultado<PerfilDeAcessoDTO>> ObterPorCodigoAsync(string codigo)
        {
            var perfil = await _perfilDeAcessoRepository.ObterPerfilPorCodigoRepositoryAsync(codigo);
            return perfil is null
                ? Resultado<PerfilDeAcessoDTO>.Falha(PerfilDeAcessoResource.Erro_RegistroNaoEncontrado)
                : Resultado<PerfilDeAcessoDTO>.Sucesso(_map.MapToDto(perfil));
        }

        public async Task<Resultado<PerfilDeAcessoUsuarioDTO>> ObterRelacionamentoAsync(string codigo)
        {
            if (string.IsNullOrEmpty(codigo))
                return Resultado<PerfilDeAcessoUsuarioDTO>.Sucesso(new PerfilDeAcessoUsuarioDTO());

            var perfil = await _perfilDeAcessoRepository.ObterPerfilPorCodigoRepositoryAsync(codigo);
            return perfil is null
                ? Resultado<PerfilDeAcessoUsuarioDTO>.Falha(PerfilDeAcessoResource.Erro_RegistroNaoEncontrado)
                : Resultado<PerfilDeAcessoUsuarioDTO>.Sucesso(_map.MapToPerfilDeAcessoUsuarioDto(perfil));
        }
    }
}
