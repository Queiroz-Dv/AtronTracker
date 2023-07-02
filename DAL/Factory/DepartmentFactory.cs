using HLP.Entity;
using PersonalTracking.Models;

namespace DAL.Factory
{
    public class DepartmentFactory : ContextBase
    {
        public DepartmentFactory()
        {
            convertObject = new ConvertObjectHelper();
        }

        internal static DEPARTMENT CreateDepartmentDal(DepartmentModel departmentModel)
        {
            return new DEPARTMENT
            {
                ID = departmentModel.DepartmentModelId,
                DepartmentName = departmentModel.DepartmentModelName
            };
        }

        internal static DepartmentModel CreateDepartmentModel(DEPARTMENT department)
        {
            return new DepartmentModel
            {
                DepartmentModelId = department.ID,
                DepartmentModelName = department.DepartmentName
            };
        }
    }
}