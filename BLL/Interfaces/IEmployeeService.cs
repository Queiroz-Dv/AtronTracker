using DAL;
using System.Collections.Generic;

namespace BLL.Interfaces
{
    public interface IEmployeeService<T> : IGenericServices<EMPLOYEE> where T : EMPLOYEE
    {
        bool IsUniqueEntity(int entity);

        IList<EMPLOYEE> GetEmployeesByUserNoAndPasswordService(int userNumber, string password);
    }
}