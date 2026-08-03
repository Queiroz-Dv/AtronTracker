using Domain.Entities;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IAprovadorObtencaoTarefaResolver
    {
        Task<Usuario> ResolverAsync(Usuario solicitante, Tarefa tarefa);
    }
}
