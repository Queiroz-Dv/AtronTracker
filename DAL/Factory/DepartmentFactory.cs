using DAL.Interfaces.FactoryModules;
using PersonalTracking.Models;
using System;

namespace DAL.Factory
{
    /// <summary>
    /// Classe factory que é utilizada para converter as entidades e models
    /// </summary>
    public class DepartmentFactory : IDepartmentFactory
    {
        protected  DepartmentModel model;

        public DepartmentFactory()
        {
            model = new DepartmentModel();
        }

        public DEPARTMENT CreateModelToDalEntity(DepartmentModel departmentModel)
        {
            return new DEPARTMENT
            {
                ID = departmentModel.DepartmentModelId,
                DepartmentName = departmentModel.DepartmentModelName
            };
        }

        public DepartmentModel CreateDalToModel(DEPARTMENT department)
        {
            return new DepartmentModel
            {
                DepartmentModelId = department.ID,
                DepartmentModelName = department.DepartmentName
            };
        }

        public DepartmentModel SetDepartmentModelFactory(DepartmentModel _entity)
        {
            if (_entity != null)
            {
                model = _entity;
                return model;
            }

            return _entity;
        }

        public DepartmentModel CreateDepartmentModelFactory()
        {
            return model;
        }
    }
}