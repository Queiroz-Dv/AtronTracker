using System.Collections.Generic;

namespace DAL.Interfaces
{
    public interface IEmployeeRepository<T> where T : EMPLOYEE
    {
        IEnumerable<T> GetUsers(int user);
    }
}
