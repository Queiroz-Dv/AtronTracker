using Atron.Domain.Entities;
using System.Threading.Tasks;

namespace Atron.Domain.Interfaces
{
    public interface ISalarioRepository : IRepository<Salario> {

        Task<Salario> ObterSalarioPorCodigoUsuario(string codigoUsuario);
    }
}