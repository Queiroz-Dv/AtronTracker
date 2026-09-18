using Application.DTO;
using Application.Mapping;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Threading.Tasks;

namespace Application.UseCases.WorkspaceCases
{
    public class ObterWorkspaceCase(WorkspaceMapping mapping, IWorkspaceRepository repository)
    {
        public async Task<Resultado<WorkspaceDTO>> ObterPorDadosDoUsuario(string codigo, string email)
        {
            var entidade = await repository.ObterWorkspacePorResponsavelEmailAsync(codigo, email);

            if (entidade.IsNullable())
            {
                return Resultado<WorkspaceDTO>.Falha("Não há vínculos com workspace para o usuário informado.");
            }

            var dto = mapping.MapToDto(entidade);

            return Resultado<WorkspaceDTO>.Sucesso(dto);
        }
    }
}