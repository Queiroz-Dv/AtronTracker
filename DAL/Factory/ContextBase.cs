using DAL.DAO;
using HLP.Helpers;
using HLP.Interfaces;

namespace DAL.Factory
{
    /// <summary>
    /// Classe padrão para os objetos do repository
    /// </summary>
    public class ContextBase<Model, Entity> 
        where Model: class 
        where Entity : class
    {
        public Context _context;

        public readonly IObjectModelHelper<Model, Entity> _objectModelHelper;

        public ContextBase(IObjectModelHelper<Model, Entity> objectModelHelper)
        {
            _context = new Context();
            _objectModelHelper = objectModelHelper;
        }

        protected QRZDatabaseDataContext GetContext()
        {
            var context = _context.GetContext();
            return context;
        }
    }
}