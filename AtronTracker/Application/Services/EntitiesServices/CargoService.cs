using Application.DTO;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared.Extensions;
using Application.Specifications.CargoSpecifications;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;

namespace Application.Services.EntitiesServices
{
    /// <summary>
    /// Classe de serviço para cargos
    /// </summary>
    public class CargoService : ICargoService
    {
        private ICargoRepository _cargoRepository;
        private IDepartamentoRepository _departamentoRepository;
        private IAsyncApplicationMapService<CargoDTO, Cargo> _map;
        private readonly IValidateModelService<Cargo> _validateModel;
        private readonly Notifiable _messageModel;

        public CargoService(IAsyncApplicationMapService<CargoDTO, Cargo> map,
                            ICargoRepository cargoRepository,
                            IDepartamentoRepository departamentoRepository,
                            IValidateModelService<Cargo> validateModel,
                            Notifiable messageModel)
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

            return await _map.MapToListDTOAsync(cargos.ToList());
        }

        public async Task<Cargo?> MontarObjetoValidado(CargoDTO cargoDTO, bool atualizando = false)
        {
            var cargo = await _map.MapToEntityAsync(cargoDTO);

            var departamento = await _departamentoRepository
                .ObterDepartamentoPorCodigoRepositoryAsync(cargoDTO.DepartamentoCodigo);
            if (departamento is null)
                _messageModel.MensagemRegistroNaoExiste(nameof(departamento));

            if (!atualizando)
            {
                var entity = await _cargoRepository.ObterCargoPorCodigoAsync(cargoDTO.Codigo);
                if (entity is not null)
                    _messageModel.AdicionarErro("Código de cargo já existente.");
            }

            if (departamento is not null)
            {
                cargo.DepartamentoId = departamento.Id;
                cargo.DepartamentoCodigo = departamento.Codigo;
            }

            _validateModel.Validate(cargo);

            return _messageModel.Notificacoes.HasErrors() ? null : cargo;
        }

        public async Task CriarAsync(CargoDTO cargoDTO)
        {
            if (cargoDTO is null)
            {
                _messageModel.MensagemRegistroInvalido();
                return;
            }

            var cargo = await MontarObjetoValidado(cargoDTO);

            if (!_messageModel.Notificacoes.HasErrors())
            {
                var criado = await _cargoRepository.CriarCargoAsync(cargo);
                if (criado)
                {
                    _messageModel.MensagemRegistroSalvo(cargo.Codigo);
                }
            }
        }

        public async Task AtualizarAsync(string codigo, CargoDTO cargoDTO)
        {
            if (!new CargoSpecification(codigo, cargoDTO.DepartamentoCodigo).IsSatisfiedBy(cargoDTO))
            {
                _messageModel.MensagemRegistroInvalido();
                return;
            }

            var cargo = await MontarObjetoValidado(cargoDTO, true);

            if (!_messageModel.Notificacoes.HasErrors())
            {
                var atualizado = await _cargoRepository.AtualizarCargoAsync(cargo);
                if (atualizado)
                {
                    _messageModel.MensagemRegistroAtualizado(cargo.Codigo);
                }
            }
        }

        public async Task RemoverAsync(string codigo)
        {
            var cargo = await _cargoRepository.ObterCargoPorCodigoAsync(codigo);
            if (cargo is not null)
            {
                await _cargoRepository.RemoverCargoAsync(cargo);
                _messageModel.MensagemRegistroRemovido(cargo.Codigo);
            }
            else
            {
                _messageModel.MensagemRegistroNaoEncontrado(codigo);
            }
        }

        public async Task<CargoDTO> ObterPorCodigoAsync(string codigo)
        {
            var cargo = await _cargoRepository.ObterCargoPorCodigoAsync(codigo);
            if (cargo is not null)
            {
                return await _map.MapToDTOAsync(cargo);
            }
            else
            {
                _messageModel.MensagemRegistroNaoEncontrado(codigo);
                return null;
            }
        }
    }
}