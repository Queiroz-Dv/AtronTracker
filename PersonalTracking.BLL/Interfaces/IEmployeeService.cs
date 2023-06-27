using PersonalTracking.DAL.DataAcess;
using System.Collections.Generic;

namespace PersonalTracking.BLL.Interfaces
{
    public interface IEmployeeService<T> : IGenericServices<EMPLOYEE> where T : EMPLOYEE
    {
        bool IsUniqueEntity(int entity);

        IList<EMPLOYEE> GetEmployeesByUserNoAndPasswordService(int userNumber, string password);
    }
}