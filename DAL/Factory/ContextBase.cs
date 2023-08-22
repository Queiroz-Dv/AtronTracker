using PersonalTracking.Helper.Interfaces;

namespace DAL.Factory
{
    /// <summary>
    /// Classe padrão para os objetos do repository
    /// </summary>
    public class ContextBase<Model, Entity>
        where Model : class
        where Entity : class
    {
        public readonly IObjectModelHelper<Model, Entity> _objectModelHelper;

        private QRZDatabaseDataContext _db;


        public ContextBase(IObjectModelHelper<Model, Entity> objectModelHelper)
        {
            _objectModelHelper = objectModelHelper;
            _db = new QRZDatabaseDataContext();
        }

        public QRZDatabaseDataContext GetContext()
        {
            return _db;
        }
    }
}