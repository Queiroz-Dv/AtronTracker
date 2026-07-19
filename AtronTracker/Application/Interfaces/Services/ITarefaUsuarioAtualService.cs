using Domain.Entities;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface ITarefaUsuarioAtualService
    {
        Task<Resultado<Usuario>> ObterAsync();
    }
}
