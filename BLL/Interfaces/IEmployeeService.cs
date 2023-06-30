using DAL;
using System.Collections.Generic;

namespace BLL.Interfaces
{
    public interface IEmployeeService : IGenericServices<EMPLOYEE>
    {
        bool IsUniqueEntity(int entity);

        IList<EMPLOYEE> GetEmployeesByUserNoAndPasswordService(int userNumber, string password);
    }
}