using Application.Resolvers;
using Domain.Interfaces;
using Domain.Tenants;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.Services.EntitiesServices.Tenancy
{
    public class DepartamentoWorkspaceService(
        WorkspaceResolver workspaceResolver, IDepartamentoRepository _departamentoRepository)
    {
        public async Task<DepartamentoWorkspace> ObterDadosTenant(string departamentoCodigo)
        {
            var workspace = await workspaceResolver.ObterWorkspaceAtual();
            var entidade = await _departamentoRepository.ObterDepartamentoPorCodigoRepository(departamentoCodigo);

            var departamentoWorkspace = new DepartamentoWorkspace()
            {
                WorkspaceId = workspace.Id,
                WorkspaceCodigo = workspace.Codigo,
                DepartamentoId = entidade.Id,
                DepartamentoCodigo = entidade.Codigo,
            };

            return departamentoWorkspace;
        }


        public async Task<Resultado> ExecutarAsync(string departamentoCodigo)
        {
            var entidade = await ObterDadosTenant(departamentoCodigo);
            var tenantGravado = await _departamentoRepository.CriarTenant(entidade);
            if (!tenantGravado)
                return Resultado.Falha(DepartamentoResource.ErroGravacao);

            return Resultado.Sucesso();
        }

        public async Task<Resultado> DesvincularAsync(string departamentoCodigo)
        {
            var entidade = await ObterDadosTenant(departamentoCodigo);
            var tenantRemovido = await _departamentoRepository.RemoverTenant(entidade);

            if (!tenantRemovido)
                return Resultado.Falha();

            return Resultado.Sucesso();
        }
    }
}