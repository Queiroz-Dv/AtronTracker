using Atron.Application.DTO;
using Atron.Application.Interfaces;
using Atron.Application.Specifications.SecuritySpecifications;
using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Shared.Extensions;
using Shared.Interfaces.Mapper;
using Shared.Interfaces.Validations;
using Shared.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.Application.Services
{
    public class ModuloService : IModuloService
    {
        private readonly IApplicationMapService<ModuloDTO, Modulo> _map;
        private readonly IModuloRepository _moduloRepository;
        private readonly IValidateModel<Modulo> _validateModel;
        private readonly MessageModel messageModel;

        public ModuloService(
            IApplicationMapService<ModuloDTO, Modulo> map,
            IModuloRepository moduloRepository,
            IValidateModel<Modulo> validateModel,
            MessageModel messageModel)
        {
            _map = map;
            _moduloRepository = moduloRepository;
            _validateModel = validateModel;
            this.messageModel = messageModel;
        }        

        private void VerificarModulo(ModuloDTO moduloDTO)
        {
            if (moduloDTO is null)
            {
                messageModel.AddRegisterInvalidMessage();
                return;
            }
        }        

        public async Task<ModuloDTO> ObterPorIdService(int id)
        {
            var entity = await _moduloRepository.ObterPorIdRepository(id);
            return _map.MapToDTO(entity);
        }

        public async Task<IEnumerable<ModuloDTO>> ObterTodosService()
        {
            var entities = await _moduloRepository.ObterTodosRepository();
            return _map.MapToListDTO(entities.ToList());
        }       

        public async Task<ModuloDTO> ObterPorCodigoService(string codigo)
        {
            var entity = await _moduloRepository.ObterPorCodigoRepository(codigo);
            return _map.MapToDTO(entity);
        }

        public List<string> ObterTodosOsCodigos()
        {
            // Removed unnecessary assignment to 'modulos' as it was not used.
            return _moduloRepository.ObterTodosRepository().Result.Select(m => m.Codigo).ToList();
        }
    }
}