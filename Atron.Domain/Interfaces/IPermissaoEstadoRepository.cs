using Atron.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Domain.Interfaces
{
    public interface IPermissaoEstadoRepository
    {
        Task<List<PermissaoEstado>> ObterTodosPermissaoEstadoRepositoryAsync();
    }
}