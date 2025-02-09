using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Application.Interfaces
{
    public interface IService<TModel> where TModel : class
    {
        Task<List<TModel>> ObterTodosServiceAsync();
    }
}