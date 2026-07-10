using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ISalarioRepository : IRepository<Salario>
    {
        Task<bool> CriarSalarioAsync(Salario entidade);

        Task<Salario> ObterSalarioPorCodigoUsuario(string codigoUsuario);

        Task<bool> AtualizarSalarioRepositoryAsync(int id, Salario salario);

        Task<Salario> ObterSalarioPorIdAsync(int id);

        Task<List<Salario>> ObterSalariosRepository();

        Task<Salario> ObterSalarioPorUsuario(int id, string codigo);        
    }
}