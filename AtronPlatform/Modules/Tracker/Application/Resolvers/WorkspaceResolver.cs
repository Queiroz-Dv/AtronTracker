using Domain.Entities;
using Domain.Interfaces.UsuarioInterfaces;
using Microsoft.AspNetCore.Http;
using Shared.Application.DTOS.Auth;
using Shared.Application.Interfaces.Service;
using Shared.Application.Services.Accessor;
using System.Threading.Tasks;

namespace Application.Resolvers
{
    public class WorkspaceResolver : UserAccessor, IUserAccessor
    {
        private readonly IWorkspaceRepository _repository;
        private readonly IHttpContextAccessor httpContextAccessor;

        public WorkspaceResolver(IHttpContextAccessor accessor, IWorkspaceRepository repository) : base(accessor)
        {
            _repository = repository;
            httpContextAccessor = accessor;
        }

        public async Task<Workspace> ObterWorkspaceAtual()
        {
            return await _repository.ObterWorkspacePorCodigo(ObterCodigoWorkspace());
        }

        public string ObterCodigoWorkspace()
        {
            var user = httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated != true)
                return string.Empty;

            return user.FindFirst(ClaimCode.CODIGO_WORKSPACE)?.Value ?? string.Empty;
        }
    }
}