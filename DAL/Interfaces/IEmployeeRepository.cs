using DAL.Generics;
using System.Collections.Generic;

namespace DAL.Interfaces
{
    public interface IEmployeeRepository : IGenericRepository<EMPLOYEE> 
    {
        IEnumerable<EMPLOYEE> GetUsers(int user);

        IEnumerable<EMPLOYEE> GetEmployeesByUserNoAndPassword(int userNumber, string password);
    }
}
