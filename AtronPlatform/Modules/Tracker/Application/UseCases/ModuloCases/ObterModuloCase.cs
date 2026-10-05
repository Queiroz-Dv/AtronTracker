using Application.DTO;
using Domain.Entities;
using Domain.Interfaces;
using Shared.Application.Interfaces.Mapping;

namespace Application.UseCases.ModuloCases
{
    public class ObterModuloCase
    {
        private readonly IToDtoMapper<Modulo, ModuloDTO> _map;
        private readonly IModuloRepository _moduloRepository;

        public ObterModuloCase(
            IToDtoMapper<Modulo, ModuloDTO> map,
            IModuloRepository moduloRepository)
        {
            _map = map;
            _moduloRepository = moduloRepository;
        }

        public async Task<ModuloDTO> ObterPorIdAsync(int id)
        {
            var entity = await _moduloRepository.ObterPorIdRepository(id);
            return _map.MapToDto(entity);
        }

        public async Task<IEnumerable<ModuloDTO>> ObterTodosAsync()
        {
            var entities = await _moduloRepository.ObterTodosRepository();
            return _map.MapToDtos(entities).ToList();
        }

        public async Task<ModuloDTO> ObterPorCodigoAsync(string codigo)
        {
            var entity = await _moduloRepository.ObterPorCodigoRepository(codigo);
            return _map.MapToDto(entity);
        }

        public List<string> ObterTodosOsCodigos()
        {
            return _moduloRepository.ObterTodosRepository().Result.Select(m => m.Codigo).ToList();
        }
    }
}
