using Atron.Application.DTO;
using Atron.Application.Interfaces;
using Atron.Application.Specifications.CargoSpecifications;
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
    /// <summary>
    /// Classe de serviço para cargos
    /// </summary>
    public class CargoService : ICargoService
    {
        private ICargoRepository _cargoRepository;
        private IDepartamentoRepository _departamentoRepository;
        private IApplicationMapService<CargoDTO, Cargo> _map;
        private readonly IValidateModel<Cargo> _validateModel;
        private readonly MessageModel _messageModel;

        public CargoService(IApplicationMapService<CargoDTO, Cargo> map,
                            ICargoRepository cargoRepository,
                            IDepartamentoRepository departamentoRepository,
                            IValidateModel<Cargo> validateModel,
                            MessageModel messageModel)
        {
            _map = map;
            _cargoRepository = cargoRepository;
            _departamentoRepository = departamentoRepository;
            _validateModel = validateModel;
            _messageModel = messageModel;
        }

        public async Task<List<CargoDTO>> ObterTodosAsync()
        {
            var cargos = await _cargoRepository.ObterCargosAsync();

            var cargosDTOs = _map.MapToListDTO(cargos.ToList());

            return cargosDTOs;
        }

        public async Task CriarAsync(CargoDTO cargoDTO)
        {
            if (cargoDTO is null)
            {
                _messageModel.AddRegisterInvalidMessage(nameof(Cargo));
                return;
            }

            var cargo = _map.MapToEntity(cargoDTO);
            var departamento = await _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsync(cargoDTO.DepartamentoCodigo);
            var entity = await _cargoRepository.ObterCargoPorCodigoAsync(cargoDTO.Codigo);

            if (entity is not null && entity.DepartamentoCodigo == cargoDTO.DepartamentoCodigo)
            {
                _messageModel.AddRegisterExistMessage(nameof(Cargo));
            }

            if (departamento is not null)
            {
                cargo.DepartmentoId = departamento.Id;
                cargo.Departamento = null;
            }

            _validateModel.Validate(cargo);

            if (!_messageModel.Messages.HasErrors())
            {
                await _cargoRepository.CriarCargoAsync(cargo);
                _messageModel.AddSuccessMessage(nameof(Cargo));
            }
        }

        public async Task AtualizarAsync(string codigo, CargoDTO cargoDTO)
        {
            if (!new CargoSpecification(codigo, cargoDTO.DepartamentoCodigo).IsSatisfiedBy(cargoDTO))
            {
                _messageModel.AddRegisterInvalidMessage(nameof(Cargo));
                return;
            }

            var cargo = _map.MapToEntity(cargoDTO);
            var departamento = await _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsync(cargoDTO.DepartamentoCodigo);

            if (departamento is not null)
            {                
                cargo.DepartmentoId = departamento.Id;
                cargo.DepartamentoCodigo = departamento.Codigo;
            }

            _validateModel.Validate(cargo);
            if (!_messageModel.Messages.HasErrors())
            {
                await _cargoRepository.AtualizarCargoAsync(cargo);
                _messageModel.AddUpdateMessage(nameof(Cargo));
            }
        }

        public async Task RemoverAsync(string codigo)
        {
            var cargo = await _cargoRepository.ObterCargoPorCodigoAsync(codigo);
            if (cargo is not null)
            {
                await _cargoRepository.RemoverCargoAsync(cargo);
                _messageModel.AddRegisterRemovedSuccessMessage(nameof(Cargo));
            }
            else
            {
                _messageModel.AddRegisterNotFoundMessage(nameof(Cargo));
            }
        }

        public async Task<CargoDTO> ObterPorCodigoAsync(string codigo)
        {
            var cargo = await _cargoRepository.ObterCargoPorCodigoAsync(codigo);
            if (cargo is not null)
            {
                return _map.MapToDTO(cargo);
            }
            else
            {
                _messageModel.AddRegisterNotFoundMessage(nameof(Cargo));
                return null;
            }
        }
    }
}