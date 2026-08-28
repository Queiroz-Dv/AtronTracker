#nullable enable

using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IEmpresaRepository
    {
        Task<Usuario?> ObterUsuarioAsync(string codigo);
        Task<bool> CodigoExisteAsync(string codigo);
        Task<UsuarioEmpresa?> ObterVinculoAsync(int usuarioId, string usuarioCodigo);
        Task CriarAsync(Empresa empresa);
    }
}

