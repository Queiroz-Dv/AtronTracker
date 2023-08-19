using PersonalTracking.Models;
using System.Collections.Generic;

namespace BLL.Interfaces
{
    public interface IDepartmentService : IGenericServices<DepartmentModel>
    {
        List<DepartmentModel> GetAllModelService();

        DepartmentModel CreateDepartmentModelObjectFactory();
    }
}