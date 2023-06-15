using DAL;

namespace BLL.Interfaces
{
    public interface IEmployeeBLL<T> : IGenericBLL<EMPLOYEE> where T : EMPLOYEE
    {
        bool isUniqueEntity(int entity);
    }
}