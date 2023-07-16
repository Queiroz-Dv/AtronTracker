using DAL.Generics;
using PersonalTracking.Models;
using System.Collections.Generic;

namespace DAL.Interfaces
{
    /// <summary>
    /// Interface do módulo de Departamento
    /// </summary>
    public interface IDepartmentRepository : IGenericRepository<DepartmentModel>
    {
        IList<DepartmentModel> GetAllDepartmentEntities();
    }
}