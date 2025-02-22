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

        public async Task CriarModuloServiceAsync(ModuloDTO moduloDTO)
        {
            VerificarModulo(moduloDTO);

            var modulo = _map.MapToEntity(moduloDTO);
            var moduloExistente = await _moduloRepository.ObterPorCodigoRepository(modulo.Codigo);

            if (new ModuloExistenteSpecification(new ModuloDTO { Codigo = moduloExistente?.Codigo }).IsSatisfiedBy(moduloDTO))
            {
                messageModel.AddRegisterExistMessage(moduloDTO.Codigo);
                return;
            }

            _validateModel.Validate(modulo);
            if (!messageModel.Messages.HasErrors())
            {
                var result = await _moduloRepository.CriarModuloRepository(modulo);
                if (result)
                {
                    messageModel.AddSuccessMessage(modulo.Codigo);
                }
            }
        }

        private void VerificarModulo(ModuloDTO moduloDTO)
        {
            if (moduloDTO is null)
            {
                messageModel.AddRegisterInvalidMessage();
                return;
            }
        }

        public async Task<ModuloDTO> AtualizarModuloServiceAsync(string codigo, ModuloDTO moduloDTO)
        {
            VerificarModulo(moduloDTO);

            var modulo = _map.MapToEntity(moduloDTO);

            if (new ModuloSpecification(codigo).IsSatisfiedBy(moduloDTO))
            {
                _validateModel.Validate(modulo);

                if (!messageModel.Messages.HasErrors())
                {
                    var result = await _moduloRepository.Atualizar(modulo);
                    if (result is not null)
                    {
                        messageModel.AddSuccessMessage(modulo.Codigo);
                    }
                    else
                    {
                        messageModel.AddRegisterNotFoundMessage(modulo.Codigo);
                    }
                }

                return moduloDTO;
            }
            else
            {
                messageModel.AddError($"O código do módulo {codigo.ToUpper()} informado difere do que foi enviado.");
                return null;
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

        // TODO: rever após implementação de perfil de acesso
        public async Task<bool> RemoverModuloServiceAsync(string codigo)
        {
            var entity = await _moduloRepository.ObterPorCodigoRepository(codigo);

            if (entity is not null)
            {
                return await _moduloRepository.RemoverModuloRepository(entity);
            }
            else
            {
                messageModel.AddRegisterNotFoundMessage(codigo);
                return false;
            }
        }

        public async Task<ModuloDTO> ObterPorCodigoService(string codigo)
        {
            var entity = await _moduloRepository.ObterPorCodigoRepository(codigo);
            return _map.MapToDTO(entity);
        }
    }
}