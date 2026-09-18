using Application.DTO;
using Application.Mapping;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Threading.Tasks;

namespace Application.UseCases.WorkspaceCases
{
    public sealed class RegistrarWorkspaceCase(
        WorkspaceMapping mapping,
        IValidador<WorkspaceDTO> validador,
        IWorkspaceRepository repository)
    {
        public async Task<Resultado> ExecutarAsync(WorkspaceDTO workspaceDTO)
        {
            var messages = validador.Validar(workspaceDTO);
            if (messages.TemErros())
                return Resultado.Falha(messages);

            var workspaceExistente = await repository.ObterWorkspacePorCodigo(workspaceDTO.Codigo);
            if (!workspaceExistente.IsNullable())
                return Resultado.Falha().ComMensagemRegistroExistente(workspaceDTO.Codigo);

            var entity = mapping.MapToEntity(workspaceDTO);
            var criado = await repository.CriarWorkspace(entity);
            if (criado)
            {
                return Resultado.Sucesso().CommMensagemRegistroSalvo(entity.Codigo);
            }

            return Resultado.Falha().ComMensagemFalhaNaCriacao(entity.Codigo);
        }
    }
}
