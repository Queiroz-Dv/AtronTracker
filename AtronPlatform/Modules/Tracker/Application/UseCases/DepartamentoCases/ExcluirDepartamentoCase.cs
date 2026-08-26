using Application.Policies.PlanejamentoCustos;
using Domain.Interfaces;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace Application.UseCases.DepartamentoCases
{
    public sealed class ExcluirDepartamentoCase(
        IDepartamentoRepository departamentoRepository,
        ICargoRepository cargoRepository,
        EstruturaPlanejadaPolicy estruturaPlanejadaPolicy,
        IUsuarioCargoDepartamentoRepository relacionamentoRepository)
    {
        private readonly IDepartamentoRepository _departamentoRepository = departamentoRepository;
        private readonly ICargoRepository _cargoRepository = cargoRepository;
        private readonly EstruturaPlanejadaPolicy _estruturaPlanejadaPolicy = estruturaPlanejadaPolicy;
        private readonly IUsuarioCargoDepartamentoRepository _relacionamentoRepository = relacionamentoRepository;

        public async Task<Resultado> ExecutarAsync(string codigo)
        {
            if (codigo.IsNullOrEmpty())
                return Resultado.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var departamento = await _departamentoRepository
                .ObterDepartamentoPorCodigoRepositoryAsync(codigo);

            if (departamento is null)
                return Resultado.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            var estruturaPlanejada = await _estruturaPlanejadaPolicy
                .ValidarRemocaoDepartamentoAsync(departamento);

            if (estruturaPlanejada.TeveFalha)
                return estruturaPlanejada;

            var relacionamentos = await _relacionamentoRepository
                .ObterPorDepartamento(departamento.Id, departamento.Codigo);

            var cargos = await _cargoRepository
                .ObterCargosPorDepartamento(departamento.Id, departamento.Codigo);

            if (relacionamentos.Any() || cargos.Any())
                return Resultado.Falha(
                    string.Format(DepartamentoResource.ErroDepartamentoContemRelacionamento, codigo));

            var removido = await _departamentoRepository
                .RemoverDepartmentoRepositoryAsync(departamento);

            if (!removido)
                return Resultado.Falha(string.Format(DepartamentoResource.ErroRemocao, codigo));

            return Resultado
                .Sucesso(departamento)
                .AdicionarMensagem(NotificacoesPadronizadas.MensagemRemocaoSucesso);
        }
    }
}
