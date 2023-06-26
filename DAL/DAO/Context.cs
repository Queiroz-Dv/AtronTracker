using System;
using System.Data.Linq;

namespace DAL.DAO
{
    public class Context : EmployeeContext, IDisposable
    {
        private EmployeeDataClassDataContext _db;

        public Context()
        {
            _db = new EmployeeDataClassDataContext();
        }

        public EmployeeDataClassDataContext GetContext()
        {
            return _db;
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}