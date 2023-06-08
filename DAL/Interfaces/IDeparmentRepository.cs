using DAL.Generics;

namespace DAL.Interfaces
{
    public interface IDeparmentRepository : IGenericRepository<DEPARTMENT>
    {
        bool ExistsDepartment(DEPARTMENT model);
    }
}