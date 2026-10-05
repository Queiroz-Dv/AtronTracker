using Application.DTO;
using Application.Interfaces.Mapping;
using Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.UseCases.PerfilDeAcessoCases
{
    public class ObterPerfisUsuarioCase
    {
        private readonly IPerfilDeAcessoMapping _map;
        private readonly IPerfilDeAcessoRepository _perfilDeAcessoRepository;

        protected ObterPerfisUsuarioCase() { }

        public ObterPerfisUsuarioCase(
            IPerfilDeAcessoMapping map,
            IPerfilDeAcessoRepository perfilDeAcessoRepository)
        {
            _map = map;
            _perfilDeAcessoRepository = perfilDeAcessoRepository;
        }

        public virtual async Task<List<PerfilDeAcessoDTO>> ExecutarAsync(string usuarioCodigo)
        {
            var perfis = await _perfilDeAcessoRepository
                .ObterPerfisPorCodigoDeUsuarioRepositoryAsync(usuarioCodigo);

            return perfis is null ? null : _map.MapToDtos(perfis).ToList();
        }
    }
}
