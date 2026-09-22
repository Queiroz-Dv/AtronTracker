using Application.Policies.PlanejamentoCustos;
using Application.Services.EntitiesServices.Tenancy;
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
        DepartamentoWorkspaceService _departamentoWorkspaceService,
        EstruturaPlanejadaPolicy _estruturaPlanejadaPolicy,
        IDepartamentoRepository _departamentoRepository,
        ICargoRepository _cargoRepository,
        IUsuarioCargoDepartamentoRepository _relacionamentoRepository)
    {       
        public async Task<Resultado> ExecutarAsync(string codigo)
        {
            if (codigo.IsNullOrEmpty())
                return Resultado.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var departamento = await _departamentoRepository
                .ObterDepartamentoPorCodigoRepositoryAsync(codigo);

            if (departamento.IsNullable())
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

            var tenantRemovido = await _departamentoWorkspaceService.DesvincularAsync(codigo);

            var removido = await _departamentoRepository
                .RemoverDepartmentoRepositoryAsync(departamento);

            if (!removido && tenantRemovido.TeveFalha)
                return Resultado.Falha(string.Format(DepartamentoResource.ErroRemocao, codigo));

            return Resultado
                .Sucesso(departamento)
                .AdicionarMensagem(NotificacoesPadronizadas.MensagemRemocaoSucesso);
        }
    }
}