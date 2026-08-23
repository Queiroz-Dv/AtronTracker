using Application.DTO;
using Application.Mapping;
using Application.Policies.PlanejamentoCustos;
using Domain.Interfaces;
using Shared.Application.Interfaces.Mapping;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace Application.UseCases.CargoCases
{
    public sealed class AtualizarCargoCase(
        IValidador<CargoDTO> validador,
        CargoMapping mapper,
        ICargoRepository cargoRepository,
        IDepartamentoRepository departamentoRepository,
        EstruturaPlanejadaPolicy estruturaPlanejadaPolicy)
    {
        private readonly IValidador<CargoDTO> _validador = validador;
        private readonly CargoMapping _mapper = mapper;
        private readonly ICargoRepository _cargoRepository = cargoRepository;
        private readonly IDepartamentoRepository _departamentoRepository = departamentoRepository;
        private readonly EstruturaPlanejadaPolicy _estruturaPlanejadaPolicy = estruturaPlanejadaPolicy;

        public async Task<Resultado> ExecutarAsync(
            string codigo,
            CargoDTO cargoDTO)
        {
            if (codigo.IsNullOrEmpty())
                return Resultado.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var erros = _validador.Validar(cargoDTO);
            if (erros.Any())
                return Resultado.Falha(erros);

            var cargo = await _cargoRepository.ObterCargoPorCodigoAsync(codigo);
            if (cargo == null)
                return Resultado.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            var departamento = await _departamentoRepository
                .ObterDepartamentoPorCodigoRepositoryAsync(cargoDTO.DepartamentoCodigo);

            if (departamento == null)
                return Resultado.Falha(CargoResource.ErroDepartamentoNaoEncontrado);

            var estruturaPlanejada = await _estruturaPlanejadaPolicy
                .ValidarMovimentacaoCargoAsync(cargo, departamento);

            if (estruturaPlanejada.TeveFalha)
                return Resultado.Falha(estruturaPlanejada.Messages);

            cargo.MapToUpdate(cargoDTO, _mapper);
            cargo.VincularDepartamento(departamento);

            if (!await _cargoRepository.AtualizarCargoAsync(cargo))
                return Resultado.Falha(
                    string.Format(CargoResource.ErroInesperadoAtualizacao, codigo));

            return Resultado
                .Sucesso()
                .AdicionarMensagem(string.Format(CargoResource.MensagemAtualizacao, codigo));
        }
    }
}
