using PersonalTracking.DAL.DataAcess;
using PersonalTracking.DAL.Generics;
using System.Collections.Generic;

namespace PersonalTracking.DAL.Interfaces
{
    public interface IEmployeeRepository<T> : IGenericRepository<T> where T : EMPLOYEE
    {
        IEnumerable<T> GetUsers(int user);

        IEnumerable<T> GetEmployeesByUserNoAndPassword(int userNumber, string password);
    }
}
