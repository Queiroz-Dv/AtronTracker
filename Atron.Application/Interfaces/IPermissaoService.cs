using Atron.Application.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Application.Interfaces
{
    public interface IPermissaoService
    {
        Task CriarPermissaoServiceAsync(PermissaoDTO permissaoDTO);

        Task<List<PermissaoDTO>> ObterTodasPermissoesServiceAsync();

        Task AtualizarPermissaoServiceAsync(PermissaoDTO permissaoDTO);

        Task ExcluirPermissaoServiceAsync(int id);
    }
}