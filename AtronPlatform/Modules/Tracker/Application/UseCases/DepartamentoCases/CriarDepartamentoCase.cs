using Application.DTO;
using Application.Mapping;
using Domain.Interfaces;
using Domain.Interfaces.UsuarioInterfaces;
using Domain.Tenants;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Threading.Tasks;

namespace Application.UseCases.DepartamentoCases
{
    public sealed class CriarDepartamentoCase(
        IUserAccessor _userAccessor,
        IWorkspaceRepository _workspaceRepository,
        VincularGestorDepartamentoCase _vincularGestorDepartamento,
        DepartamentoMapping _mapper,
        IDepartamentoRepository _departamentoRepository,
        IValidador<DepartamentoDTO> _validador)
    {
        public async Task<Resultado> ExecutarAsync(DepartamentoDTO departamentoDTO)
        {
            var messages = _validador.Validar(departamentoDTO);
            if (messages.TemErros())
                return Resultado.Falha(messages);

            var departamentoExiste = await _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsync(departamentoDTO.Codigo);

            if (!departamentoExiste.IsNullable())
                return Resultado.Falha(DepartamentoResource.ErroCodigoDepartamentoExistente);

            var departamento = _mapper.MapToEntity(departamentoDTO);

            var resultadoGestor = await _vincularGestorDepartamento
                .ExecutarAsync(departamento, departamentoDTO.GestorDepartamentoCodigo);

            if (resultadoGestor.TeveFalha)
                return Resultado.Falha(resultadoGestor.Messages);

            var entidadeGravada = await _departamentoRepository.CriarDepartamentoRepositoryAsync(departamento);
            if (!entidadeGravada) return Resultado.Falha(DepartamentoResource.ErroGravacao);

            var dadosWorkspace = _userAccessor.ObterDadosDoTenant();
            var entidade = await _departamentoRepository.ObterDepartamentoPorCodigoRepository(departamentoDTO.Codigo);
            var workspace = await _workspaceRepository.ObterWorkspacePorCodigo(dadosWorkspace.CodigoWorkspace);
            var tenant = new DepartamentoWorkspace()
            {
                WorkspaceId = workspace.Id,
                WorkspaceCodigo = workspace.Codigo,
                DepartamentoId = entidade.Id,
                DepartamentoCodigo = entidade.Codigo,
            };

            var tenantGravado = await _departamentoRepository.CriarTenant(tenant);
            if (!tenantGravado) return Resultado.Falha(DepartamentoResource.ErroGravacao);

            return Resultado
                .Sucesso()
                .AdicionarMensagem(string.Format(
                    NotificacoesPadronizadas.Mensagem_RegistroSalvo,
                    departamento.Codigo));
        }
    }
}