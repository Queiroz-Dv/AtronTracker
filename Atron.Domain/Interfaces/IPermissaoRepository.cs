using Atron.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Domain.Interfaces
{
    public interface IPermissaoRepository
    {
        Task<Permissao> CriarPermissaoRepositoryAsync(Permissao permissao);
        Task<IEnumerable<Permissao>> ObterPermissoesRepositoryAsync();
    }
}