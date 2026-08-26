using Application.DTO;
using Application.Mapping;
using Domain.Interfaces;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using System.Linq;
using System.Threading.Tasks;

namespace Application.UseCases.CargoCases
{
    public sealed class CriarCargoCase(
        IValidador<CargoDTO> validador,
        CargoMapping mapper,
        ICargoRepository cargoRepository,
        IDepartamentoRepository departamentoRepository)
    {
        private readonly IValidador<CargoDTO> _validador = validador;
        private readonly CargoMapping _mapper = mapper;
        private readonly ICargoRepository _cargoRepository = cargoRepository;
        private readonly IDepartamentoRepository _departamentoRepository = departamentoRepository;

        public async Task<Resultado> ExecutarAsync(CargoDTO cargoDTO)
        {
            var erros = _validador.Validar(cargoDTO);
            if (erros.Any())
                return Resultado.Falha(erros.First());

            var cargoExiste = await _cargoRepository.ObterCargoPorCodigoAsync(cargoDTO.Codigo);
            if (cargoExiste != null)
                return Resultado.Falha(CargoResource.ErroCodigoCargoExistente);

            var departamento = await _departamentoRepository
                .ObterDepartamentoPorCodigoRepositoryAsync(cargoDTO.DepartamentoCodigo);

            if (departamento == null)
                return Resultado.Falha(CargoResource.ErroDepartamentoNaoEncontrado);

            var cargo = _mapper.MapToEntity(cargoDTO);
            cargo.VincularDepartamento(departamento);

            if (!await _cargoRepository.CriarCargoAsync(cargo))
                return Resultado.Falha(CargoResource.ErroGravacao);

            return Resultado
                .Sucesso()
                .AdicionarMensagem(string.Format(
                    NotificacoesPadronizadas.ResourceManager.GetString("Mensagem_RegistroSalvo")!,
                    cargoDTO.Codigo));
        }
    }
}
