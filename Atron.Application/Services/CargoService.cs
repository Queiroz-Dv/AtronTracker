using Atron.Application.DTO;
using Atron.Application.Interfaces;
using Atron.Application.Specifications.CargoSpecifications;
using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using AutoMapper;
using Shared.Extensions;
using Shared.Interfaces.Validations;
using Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Application.Services
{
    /// <summary>
    /// Classe de serviço para cargos
    /// </summary>
    public class CargoService : ICargoService
    {
        private readonly IMapper _mapper;
        private ICargoRepository _cargoRepository;
        private IDepartamentoRepository _departamentoRepository;
        private readonly IValidateModel<Cargo> _validateModel;
        private readonly MessageModel _messageModel;

        public CargoService(IMapper mapper,
                            ICargoRepository cargoRepository,
                            IDepartamentoRepository departamentoRepository,
                            MessageModel messageModel,
                            IValidateModel<Cargo> validateModel)
        {
            _mapper = mapper;
            _cargoRepository = cargoRepository;
            _departamentoRepository = departamentoRepository;
            _messageModel = messageModel;
            _validateModel = validateModel;
        }

        public async Task<List<CargoDTO>> ObterTodosAsync()
        {
            var cargos = await _cargoRepository.ObterCargosAsync();

            var cargosDTOs = _mapper.Map<List<CargoDTO>>(cargos);

            //cargosDTOs = (from crg in cargosDTOs
            //              select new CargoDTO(crg.Codigo, crg.Descricao, new DepartamentoDTO(crg.Departamento.Codigo, ))
            //              {
            //                  Codigo = crg.Codigo,
            //                  Descricao = crg.Descricao,
            //                  DepartamentoCodigo = crg.DepartamentoCodigo,
            //                  Departamento =  { Codigo = crg.Departamento.Codigo, Descricao = crg.Departamento.Descricao }
            //              }).ToList();

            return cargosDTOs;
        }

        public async Task CriarAsync(CargoDTO cargoDTO)
        {
            if (cargoDTO is null)
            {
                _messageModel.AddRegisterInvalidMessage(nameof(Cargo));
                return;
            }

            var cargo = _mapper.Map<Cargo>(cargoDTO);
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

            var cargo = _mapper.Map<Cargo>(cargoDTO);
            var departamento = _departamentoRepository.DepartamentoExiste(cargoDTO.DepartamentoCodigo);

            if (departamento)
            {
                var entidade = await _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsync(cargoDTO.DepartamentoCodigo);
                cargo.DepartmentoId = entidade.Id;

                if (cargo.Departamento is not null)
                    cargo.Departamento = null;

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
                return _mapper.Map<CargoDTO>(cargo);
            }
            else
            {
                _messageModel.AddRegisterNotFoundMessage(nameof(Cargo));
                return null;
            }
        }
    }
}