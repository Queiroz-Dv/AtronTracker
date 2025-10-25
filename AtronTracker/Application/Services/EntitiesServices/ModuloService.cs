using Application.DTO;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Interfaces;
using Shared.Interfaces.Mapper;
using Shared.Interfaces.Validations;
using Shared.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services.EntitiesServices
{
    public class ModuloService : IModuloService
    {
        private readonly IAsyncApplicationMapService<ModuloDTO, Modulo> _map;
        private readonly IModuloRepository _moduloRepository;
        private readonly IValidateModel<Modulo> _validateModel;
        private readonly MessageModel messageModel;

        public ModuloService(
            IAsyncApplicationMapService<ModuloDTO, Modulo> map,
            IModuloRepository moduloRepository,
            IValidateModel<Modulo> validateModel,
            MessageModel messageModel)
        {
            _map = map;
            _moduloRepository = moduloRepository;
            _validateModel = validateModel;
            this.messageModel = messageModel;
        }

        public async Task<ModuloDTO> ObterPorIdService(int id)
        {
            var entity = await _moduloRepository.ObterPorIdRepository(id);
            return await _map.MapToDTOAsync(entity);
        }

        public async Task<IEnumerable<ModuloDTO>> ObterTodosService()
        {
            var entities = await _moduloRepository.ObterTodosRepository();
            return await _map.MapToListDTOAsync(entities.ToList());
        }

        public async Task<ModuloDTO> ObterPorCodigoService(string codigo)
        {
            var entity = await _moduloRepository.ObterPorCodigoRepository(codigo);
            return await _map.MapToDTOAsync(entity);
        }

        public List<string> ObterTodosOsCodigos()
        {
            return _moduloRepository.ObterTodosRepository().Result.Select(m => m.Codigo).ToList();
        }
    }
}