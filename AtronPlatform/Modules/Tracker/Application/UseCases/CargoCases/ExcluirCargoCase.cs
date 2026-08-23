using Application.Policies.PlanejamentoCustos;
using Domain.Interfaces;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace Application.UseCases.CargoCases
{
    public sealed class ExcluirCargoCase(
        ICargoRepository cargoRepository,
        EstruturaPlanejadaPolicy estruturaPlanejadaPolicy,
        IUsuarioCargoDepartamentoRepository relacionamentoRepository)
    {
        private readonly ICargoRepository _cargoRepository = cargoRepository;
        private readonly EstruturaPlanejadaPolicy _estruturaPlanejadaPolicy = estruturaPlanejadaPolicy;
        private readonly IUsuarioCargoDepartamentoRepository _relacionamentoRepository = relacionamentoRepository;

        public async Task<Resultado> ExecutarAsync(string codigo)
        {
            if (codigo.IsNullOrEmpty())
                return Resultado.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var cargo = await _cargoRepository.ObterCargoPorCodigoAsync(codigo);
            if (cargo == null)
                return Resultado.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            var estruturaPlanejada = await _estruturaPlanejadaPolicy
                .ValidarRemocaoCargoAsync(cargo);

            if (estruturaPlanejada.TeveFalha)
                return estruturaPlanejada;

            var relacionamentos = await _relacionamentoRepository
                .ObterPorCargo(cargo.Id, cargo.Codigo);

            if (relacionamentos.Any())
                return Resultado.Falha(
                    string.Format(CargoResource.ErroCargoContemRelacionamento, codigo));

            if (!await _cargoRepository.RemoverCargoAsync(cargo))
                return Resultado.Falha(string.Format(CargoResource.ErroRemocao, codigo));

            return Resultado
                .Sucesso(cargo)
                .AdicionarMensagem(NotificacoesPadronizadas.MensagemRemocaoSucesso);
        }
    }
}
