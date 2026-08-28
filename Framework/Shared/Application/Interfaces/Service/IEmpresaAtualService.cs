using Shared.Application.DTOS.Empresas;

namespace Shared.Application.Interfaces.Service;

public interface IEmpresaAtualService
{
    Task<ContextoEmpresa> ObterAsync();
}
