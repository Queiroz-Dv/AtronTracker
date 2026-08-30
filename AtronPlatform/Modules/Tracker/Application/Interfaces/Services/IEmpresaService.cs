using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTO;
using Shared.Domain.ValueObjects;

namespace Application.Interfaces.Services
{
    public interface IEmpresaService
    {
        Task<Resultado<EmpresaDTO>> CriarAsync(EmpresaDTO empresa);
        Task<Resultado<IReadOnlyList<EmpresaDTO>>> ObterTodosAsync();
        Task<Resultado<EmpresaDTO>> ObterPorCodigoAsync(string codigo);
        Task<Resultado<EmpresaDTO>> AtualizarAsync(string codigo, EmpresaDTO empresa);
        Task<Resultado> RemoverAsync(string codigo);
    }
}
