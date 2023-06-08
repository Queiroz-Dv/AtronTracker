using DAL.DTO;
using System.Collections.Generic;

namespace BLL.Services
{
    public interface IDepartmentServices
    {
        ICollection<DepartmentDTO> GetAllDepartmentsService();

        DepartmentDTO GetDepartmentByIdService(int? id);

        DepartmentDTO CreateDepartmentServices(DepartmentDTO department);

        DepartmentDTO UpdateDepartmentService(DepartmentDTO department);

        DepartmentDTO DeleteDepartmentByIdService(DepartmentDTO department);

        void DeleteAllDepartmentsService(ICollection<DepartmentDTO> department);
    }
}
