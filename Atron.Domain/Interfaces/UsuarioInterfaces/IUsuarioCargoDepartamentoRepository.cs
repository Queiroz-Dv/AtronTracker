using Atron.Domain.Entities;
using System.Threading.Tasks;

namespace Atron.Domain.Interfaces.UsuarioInterfaces
{
    public interface IUsuarioCargoDepartamentoRepository
    {
        public Task<bool> GravarAssociacaoUsuarioCargoDepartamento(Usuario usuario, Cargo cargo, Departamento departamento);
    }
}