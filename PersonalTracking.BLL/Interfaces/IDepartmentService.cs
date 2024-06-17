using Entities;
using Factory.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PersonalTracking.BLL.Interfaces
{
    public interface IDepartmentService : IBaseNotificationService
    {
        Task<IEnumerable<DepartmentDto>> GetDepartments();

        Task<DepartmentDto> GetById(int? id);

        Task<DepartmentDto> Create(DepartmentDto department);

        Task<DepartmentDto> Update(DepartmentDto department);

        Task<DepartmentDto> Remove(DepartmentDto department);
    }
}