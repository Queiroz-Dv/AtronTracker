using Atron.Application.Interfaces.Services;
using Atron.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.Application.Services.EntitiesServices
{
    public class Service<TModel> : IService<TModel> where TModel : class
    {
        public readonly IRepository<TModel> _repository;

        public Service(IRepository<TModel> repository)
        {
            _repository = repository;
        }

        public async Task<List<TModel>> ObterTodosServiceAsync()
        {
            var entities = await _repository.ObterTodosRepositoryAsync();
            return entities.ToList();
        }
    }
}