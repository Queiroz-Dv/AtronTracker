using System;
using System.Data.Linq;

namespace DAL.DAO
{
    public class Context : EmployeeContext, IDisposable
    {
        public EmployeeDataClassDataContext _db = new EmployeeDataClassDataContext();

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