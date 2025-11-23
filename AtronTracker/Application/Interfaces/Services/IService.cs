using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IService<TModel> where TModel : class
    {
        Task<List<TModel>> ObterTodosServiceAsync();
    }
}