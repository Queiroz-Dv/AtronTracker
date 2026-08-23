using Application.DTO;
using Application.Interfaces.Mapping;
using Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.UseCases.PerfilDeAcessoCases
{
    public sealed class ObterPerfisUsuarioCase(
        IPerfilDeAcessoMapping map,
        IPerfilDeAcessoRepository perfilDeAcessoRepository)
    {
        private readonly IPerfilDeAcessoMapping _map = map;
        private readonly IPerfilDeAcessoRepository _perfilDeAcessoRepository = perfilDeAcessoRepository;

        public async Task<List<PerfilDeAcessoDTO>> ExecutarAsync(string usuarioCodigo)
        {
            var perfis = await _perfilDeAcessoRepository
                .ObterPerfisPorCodigoDeUsuarioRepositoryAsync(usuarioCodigo);

            return perfis is null ? null : _map.MapToDtos(perfis).ToList();
        }
    }
}
