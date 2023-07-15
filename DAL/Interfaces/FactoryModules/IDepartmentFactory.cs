using PersonalTracking.Models;

namespace DAL.Interfaces.FactoryModules
{
    public interface IDepartmentFactory
    {
        DEPARTMENT CreateDepartmentDal(DepartmentModel model);

        DepartmentModel CreateDepartmentModel(DEPARTMENT entity);
    }
}
