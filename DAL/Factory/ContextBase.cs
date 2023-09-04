using PersonalTracking.Entities;
using PersonalTracking.Factory.Interfaces;

namespace DAL.Factory
{
    /// <summary>
    /// Classe padrão para os objetos do repository
    /// </summary>
    public class ContextBase<Model, Entity>
        where Model : class
        where Entity : class
    {
        public IModelFactory<Model, Entity> _factory;

        private QRZDatabaseDataContext _db;

        public ContextBase(IModelFactory<Model, Entity> factory)
        {
            _factory = factory;
            _db = new QRZDatabaseDataContext();
        }

        public QRZDatabaseDataContext GetContext()
        {
            return _db;
        }
    }
}