using DAL;

namespace BLL.Interfaces
{
    public interface IDepartmentService<T> : IGenericServices<DEPARTMENT> where T : DEPARTMENT
    {

    }
}