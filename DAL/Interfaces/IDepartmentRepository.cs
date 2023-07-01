using DAL.Generics;
using PersonalTracking.Models;
using System.Collections.Generic;

namespace DAL.Interfaces
{
    public interface IDepartmentRepository : IGenericRepository<DepartmentModel>
    {
        List<DEPARTMENT> GetAllDepartmentEntities();
    }
}
