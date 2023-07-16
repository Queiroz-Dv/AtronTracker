using DAL.Interfaces.FactoryModules;
using PersonalTracking.Models;

namespace DAL.Factory
{
    /// <summary>
    /// Classe factory que é utilizada para converter as entidades e models
    /// </summary>
    public class DepartmentFactory : IDepartmentFactory
    {
        public DEPARTMENT CreateDalEntity(DepartmentModel departmentModel)
        {
            return new DEPARTMENT
            {
                ID = departmentModel.DepartmentModelId,
                DepartmentName = departmentModel.DepartmentModelName
            };
        }

        public DepartmentModel CreateModel(DEPARTMENT department)
        {
            return new DepartmentModel
            {
                DepartmentModelId = department.ID,
                DepartmentModelName = department.DepartmentName
            };
        }
    }
}