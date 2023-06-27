using PersonalTracking.DAL.DataAcess;
using PersonalTracking.DAL.Generics;

namespace PersonalTracking.DAL.Interfaces
{
    public interface IDepartmentRepository<T> : IGenericRepository<T> where T : DEPARTMENT
    {
    }
}
