using Atron.Application.DTO;
using Atron.Application.Interfaces;
using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Shared.Extensions;
using AutoMapper;
using Notification.Models;
using Shared.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atron.Application.Specifications.Cargo;

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
        private readonly MessageModel<Cargo> messageModel;

        public CargoService(IMapper mapper,
                            ICargoRepository cargoRepository,
                            IDepartamentoRepository departamentoRepository,
                            MessageModel<Cargo> messageModel)
        {
            _mapper = mapper;
            _cargoRepository = cargoRepository;
            _departamentoRepository = departamentoRepository;
            this.messageModel = messageModel;
        }

        public async Task<List<CargoDTO>> ObterTodosAsync()
        {
            var cargos = await _cargoRepository.ObterCargosAsync();
            var departamentos = await _departamentoRepository.ObterDepartmentosAsync();

            var cargosDTOs = _mapper.Map<IEnumerable<CargoDTO>>(cargos);
            var departamentosDTOs = _mapper.Map<IEnumerable<DepartamentoDTO>>(departamentos);

            var cargosComDepartamentos = (from pst in cargosDTOs
                                          join dpt in departamentosDTOs on pst.DepartamentoCodigo equals dpt.Codigo
                                          select new CargoDTO
                                          {
                                              Codigo = pst.Codigo,
                                              Descricao = pst.Descricao,
                                              DepartamentoCodigo = dpt.Codigo,
                                              Departamento = new DepartamentoDTO() { Codigo = dpt.Codigo, Descricao = dpt.Descricao }
                                          }).ToList();

            return cargosComDepartamentos;
        }

        public async Task CriarAsync(CargoDTO cargoDTO)
        {
            if (cargoDTO is null)
            {
                messageModel.AddRegisterInvalidMessage(nameof(Cargo));
                return;
            }

            var cargo = _mapper.Map<Cargo>(cargoDTO);
            var departamento = await _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsync(cargoDTO.DepartamentoCodigo);
            var entity = await _cargoRepository.ObterCargoPorCodigoAsync(cargoDTO.Codigo);

            if (entity is not null && entity.DepartmentoCodigo == cargoDTO.DepartamentoCodigo)
            {
                messageModel.AddRegisterExistMessage(nameof(Cargo));
            }

            if (departamento is not null)
            {
                cargo.DepartmentoId = departamento.Id;
                cargo.Departamento = null;
            }

            messageModel.Validate(cargo);

            if (!messageModel.Messages.HasErrors())
            {
                await _cargoRepository.CriarCargoAsync(cargo);
                messageModel.AddSuccessMessage(nameof(Cargo));
            }
        }

        public async Task AtualizarAsync(string codigo, CargoDTO cargoDTO)
        {
            if (!new CargoSpecification(codigo, cargoDTO.DepartamentoCodigo).IsSatisfiedBy(cargoDTO))
            {
                messageModel.AddRegisterInvalidMessage(nameof(Cargo));
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

            messageModel.Validate(cargo);
            if (!messageModel.Messages.HasErrors())
            {
                await _cargoRepository.AtualizarCargoAsync(cargo);
                messageModel.AddUpdateMessage(nameof(Cargo));
            }
        }

        public async Task RemoverAsync(string codigo)
        {
            var cargo = await _cargoRepository.ObterCargoPorCodigoAsync(codigo);
            if (cargo is not null)
            {
                await _cargoRepository.RemoverCargoAsync(cargo);
                messageModel.AddRegisterRemovedSuccessMessage(nameof(Cargo));
            }
            else
            {
                messageModel.AddRegisterNotFoundMessage(nameof(Cargo));
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
                messageModel.AddRegisterNotFoundMessage(nameof(Cargo));
                return null;
            }            
        }

        public IList<Message> GetMessages()
        {
            return messageModel.Messages;
        }
    }
}