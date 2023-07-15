using DAL.Interfaces.FactoryModules;
using PersonalTracking.Models;

namespace DAL.Factory
{
    public class DepartmentFactory : IDepartmentFactory
    {
        public DEPARTMENT CreateDepartmentDal(DepartmentModel departmentModel)
        {
            return new DEPARTMENT
            {
                ID = departmentModel.DepartmentModelId,
                DepartmentName = departmentModel.DepartmentModelName
            };
        }

        public DepartmentModel CreateDepartmentModel(DEPARTMENT department)
        {
            return new DepartmentModel
            {
                DepartmentModelId = department.ID,
                DepartmentModelName = department.DepartmentName
            };
        }
    }
}