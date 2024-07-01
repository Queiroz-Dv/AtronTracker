using Atron.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Domain.Interfaces
{
    public interface ISalarioRepository
    {
        Task<Salario> CriarSalarioRepositoryAsync(Salario salario);
        Task<IEnumerable<Salario>> ObterSalariosRepositoryAsync();
    }
}