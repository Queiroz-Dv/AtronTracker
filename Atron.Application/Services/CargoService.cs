using Atron.Application.DTO;
using Atron.Application.Interfaces;
using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.Application.Services
{
    public class CargoService : ICargoService
    {
        private ICargoRepository _cargoRepository;
        private IDepartamentoRepository _departamentoRepository;

        private readonly IMapper _mapper;


        public CargoService(IMapper mapper, ICargoRepository cargoRepository, IDepartamentoRepository departamentoRepository)
        {
            _cargoRepository = cargoRepository;
            _departamentoRepository = departamentoRepository;
            _mapper = mapper;
        }

        public async Task<List<CargoDTO>> ObterTodosAsync()
        {
            var cargos = await _cargoRepository.ObterCargosAsync();
            var departamentos = await _departamentoRepository.ObterDepartmentosAsync();

            var cargosDTOs = ObterCargosMapeados(cargos);
            var departamentosDTOs = ObterDepartamentosMapeados(departamentos);

            var cargosComDepartamentos = (from pst in cargosDTOs
                                          join dpt in departamentosDTOs on pst.DepartamentoCodigo equals dpt.Codigo
                                          select new CargoDTO
                                          {
                                              Codigo = pst.Codigo,
                                              Descricao = pst.Descricao,
                                              DepartamentoCodigo = dpt.Codigo,
                                              DepartamentoDescricao = dpt.Descricao
                                          }).ToList();

            return cargosComDepartamentos;
        }

        public async Task CriarAsync(CargoDTO cargoDTO)
        {
            var cargo = _mapper.Map<Cargo>(cargoDTO);
            var departamento = await _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsync(cargoDTO.DepartamentoCodigo);

            if (departamento is not null)
            {
                cargo.DepartmentoId = departamento.Id;
            }

            //GuardianModel.Validate(cargo);

            //if (GuardianModel.HasErrors() is not true)
            //{
            //    await _cargoRepository.CriarCargoAsync(cargo);
            //}
        }

        public async Task AtualizarAsync(CargoDTO cargoDTO)
        {
            var cargo = _mapper.Map<Cargo>(cargoDTO);
            var departamento = await _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsync(cargoDTO.DepartamentoCodigo);

            if (departamento is not null)
            {
                cargo.DepartmentoId = cargoDTO.DepartamentoId;
            }
            await _cargoRepository.AtualizarCargoAsync(cargo);
        }

        public async Task RemoverAsync(int? id)
        {
            var cargo = _cargoRepository.ObterCargoPorIdAsync(id).Result;
            await _cargoRepository.RemoverCargoAsync(cargo);
        }

        private IEnumerable<CargoDTO> ObterCargosMapeados(IEnumerable<Cargo> cargos) => _mapper.Map<IEnumerable<CargoDTO>>(cargos);

        private IEnumerable<DepartamentoDTO> ObterDepartamentosMapeados(IEnumerable<Departamento> departamentos) => _mapper.Map<IEnumerable<DepartamentoDTO>>(departamentos);

        public async Task<CargoDTO> ObterPorCodigoAsync(string codigo)
        {
            var cargo = await _cargoRepository.ObterCargoPorCodigoAsync(codigo);

            var cargoDTO = _mapper.Map<CargoDTO>(cargo);

            return cargoDTO;
        }
    }
}