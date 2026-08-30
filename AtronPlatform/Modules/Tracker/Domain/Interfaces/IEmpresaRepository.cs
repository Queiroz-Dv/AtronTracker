#nullable enable
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IEmpresaRepository
    {
        Task<IReadOnlyList<Empresa>> ObterTodosAsync();
        Task<Empresa?> ObterPorCodigoAsync(string codigo, bool rastrear = false);
        Task<bool> CodigoExisteAsync(string codigo, int? empresaIdIgnorada = null);
        Task<bool> CriarAsync(Empresa empresa);
        Task<bool> AtualizarAsync(Empresa empresa);
        Task<bool> RemoverAsync(Empresa empresa);
    }
}
