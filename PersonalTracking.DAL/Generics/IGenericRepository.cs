using System.Linq;

namespace PersonalTracking.DAL.Generics
{
    public interface IGenericRepository<TModel> where TModel : class
    {
        IQueryable<TModel> GetEntities();

        TModel GetById(int id);

        void Add(TModel model);

        void Update(TModel entity);

        void Delete(TModel entity);
    }
}