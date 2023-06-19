using DAL.Generics;

namespace DAL.Interfaces
{
    public interface IDepartmentRepository<T> : IGenericRepository<T> where T : DEPARTMENT
    {
    }
}
