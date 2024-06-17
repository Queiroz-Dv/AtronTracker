using Factory.Interfaces;
using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PersonalTracking.DAL.Interfaces
{
    /// <summary>
    /// Interface do módulo de Departamento
    /// </summary>
    public interface IDepartmentRepository : IBaseNotificationService
    {
        Task<IEnumerable<Department>> GetDepartments();

        Task<Department> GetDepartmentById(int? id);

        Task<Department> CreateDepartment(Department department);

        Task<Department> UpdateDepartment(Department department);

        Task<Department> RemoveDepartment(Department department);
    }
}