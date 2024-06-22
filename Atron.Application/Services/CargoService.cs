using Atron.Application.DTO;
using Atron.Application.Interfaces;
using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using AutoMapper;
using Notification.Models;
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
        private readonly IMapper _mapper;
        private ICargoRepository _cargoRepository;
        private IDepartamentoRepository _departamentoRepository;
        private readonly NotificationModel<Cargo> _notification;

        public List<NotificationMessage> notificationMessages { get; set; }


        public CargoService(IMapper mapper,
                            ICargoRepository cargoRepository,
                            IDepartamentoRepository departamentoRepository,
                            NotificationModel<Cargo> notification)
        {
            _cargoRepository = cargoRepository;
            _departamentoRepository = departamentoRepository;
            _mapper = mapper;
            notificationMessages = new List<NotificationMessage>();
            _notification = notification;
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
            cargoDTO.Id = cargoDTO.GerarIdentificador();
            var cargo = _mapper.Map<Cargo>(cargoDTO);
            var departamento = await _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsync(cargoDTO.DepartamentoCodigo);

            if (departamento is not null)
            {
                // Preciso obter o identificador do departamento através do código pra informar aqui
                cargo.DepartmentoId = departamento.Id;
            }

            _notification.Validate(cargo);

            if (!_notification.Messages.HasErrors())
            {
                await _cargoRepository.CriarCargoAsync(cargo);
                notificationMessages.Add(new NotificationMessage("Cargo criado com sucesso."));
            }

            notificationMessages.AddRange(_notification.Messages);
        }

        public async Task AtualizarAsync(CargoDTO cargoDTO)
        {
            var cargo = _mapper.Map<Cargo>(cargoDTO);
            var departamento = _departamentoRepository.DepartamentoExiste(cargoDTO.DepartamentoCodigo);

            if (departamento)
            {
                var entidade= await _cargoRepository.ObterCargoPorCodigoAsync(cargoDTO.Codigo);
                cargo.DepartmentoId = entidade.DepartmentoId;
                cargo.SetId(entidade.Id); // Atribuição de Id internamente
            }

            _notification.Validate(cargo);
            if (!_notification.Messages.HasErrors())
            {
                await _cargoRepository.AtualizarCargoAsync(cargo);
                notificationMessages.Add(new NotificationMessage("Cargo atualizado com sucesso."));
            }
        }

        public async Task RemoverAsync(int? id)
        {
            var cargo = await _cargoRepository.ObterCargoPorIdAsync(id);
            await _cargoRepository.RemoverCargoAsync(cargo);
        }

        public async Task<CargoDTO> ObterPorCodigoAsync(string codigo)
        {
            var cargo = await _cargoRepository.ObterCargoPorCodigoAsync(codigo);

            var cargoDTO = _mapper.Map<CargoDTO>(cargo);

            return cargoDTO;
        }
    }
}