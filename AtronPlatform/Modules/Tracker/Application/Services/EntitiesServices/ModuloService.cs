using Application.DTO;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Interfaces;
using Shared.Application.Interfaces.Mapping;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services.EntitiesServices
{
    public class ModuloService : IModuloService
    {
        private readonly IToDtoMapper<Modulo, ModuloDTO> _map;
        private readonly IModuloRepository _moduloRepository;
        private readonly IValidateModelService<Modulo> _validateModel;
        private readonly Notifiable messageModel;

        public ModuloService(
            IToDtoMapper<Modulo, ModuloDTO> map,
            IModuloRepository moduloRepository,
            IValidateModelService<Modulo> validateModel,
            Notifiable messageModel)
        {
            _map = map;
            _moduloRepository = moduloRepository;
            _validateModel = validateModel;
            this.messageModel = messageModel;
        }

        public async Task<ModuloDTO> ObterPorIdService(int id)
        {
            var entity = await _moduloRepository.ObterPorIdRepository(id);
            return _map.MapToDto(entity);
        }

        public async Task<IEnumerable<ModuloDTO>> ObterTodosService()
        {
            var entities = await _moduloRepository.ObterTodosRepository();
            return _map.MapToDtos(entities).ToList();
        }

        public async Task<ModuloDTO> ObterPorCodigoService(string codigo)
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
