using DAL.Generics;
using PersonalTracking.Models;

namespace DAL.Interfaces
{
    /// <summary>
    /// Interface do módulo de Departamento
    /// </summary>
    public interface IDepartmentRepository : IGenericRepository<DepartmentModel>
    { }
}