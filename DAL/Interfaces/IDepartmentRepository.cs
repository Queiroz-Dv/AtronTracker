using DAL.Generics;
using System.Collections.Generic;

namespace DAL.Interfaces
{
    public interface IDepartmentRepository : IGenericRepository<DEPARTMENT>
    {
        List<DEPARTMENT> GetAllDepartmentEntities();
    }
}
