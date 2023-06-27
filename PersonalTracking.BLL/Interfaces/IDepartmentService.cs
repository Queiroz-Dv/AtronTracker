using PersonalTracking.DAL.DataAcess;

namespace PersonalTracking.BLL.Interfaces
{
    public interface IDepartmentService<T> : IGenericServices<DEPARTMENT> where T : DEPARTMENT
    {

    }
}