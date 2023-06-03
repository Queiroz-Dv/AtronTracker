using System;
using System.Data.Linq;

namespace DAL.DAO
{
    public class Context : IDisposable
    {
        private readonly EmployeeDataClassDataContext _db;

        //public static EmployeeDataClassDataContext db = new EmployeeDataClassDataContext();

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

        public Table<DEPARTMENT> Departments { get; set; }
    }
}
