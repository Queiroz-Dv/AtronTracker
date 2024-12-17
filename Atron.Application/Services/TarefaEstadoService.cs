using Atron.Application.Interfaces;
using Atron.Domain.Entities;
using Atron.Domain.Interfaces;

namespace Atron.Application.Services
{
    public class TarefaEstadoService : Service<TarefaEstado>, ITarefaEstadoService
    {
        public TarefaEstadoService(IRepository<TarefaEstado> repository) : base(repository)
        { }
    }
}
