using DAL.DAO;
using HLP.Interfaces;

namespace DAL.Factory
{
    public class ContextBase
    {
        public Context _context;

        public IConvertObjectHelper convertObject;

        protected EmployeeDataClassDataContext GetContext()
        {
            _context = new Context();
            var context = _context.GetContext();
            return context;
        }
    }
}