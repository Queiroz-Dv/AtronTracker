using BLL.Interfaces;
using DAL;
using DAL.Interfaces;
using HLP.Entity;
using HLP.Interfaces;
using PersonalTracking.Models;
using System.Collections.Generic;
using System.Linq;

namespace BLL.Services
{
    public class DepartmentService : IDepartmentService
    {
        //171920
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IEntityMessages entityMessages;
        private readonly DEPARTMENT _department;
        private  DepartmentModel departmentModel;

        public DepartmentService(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
            entityMessages = new InformationMessage();
            _department = new DEPARTMENT();
            departmentModel = new DepartmentModel(entityMessages);
        }

        public DepartmentModel CreateEntityService(DepartmentModel entity)
        {
            var _departmentEntity = SetDepartmentEntity(entity);
            var departmentModelSetter = SetDepartmentModel(_departmentEntity);

            departmentModelSetter.Validate();
            if (!departmentModelSetter.IsValidModel)
            {
                foreach (DepartmentModel item in departmentModelSetter.Errors)
                {
                    item.ShowMessageBoxErrors();
                }
            }
            else
            {
                _departmentRepository.CreateEntityRepository(_departmentEntity);
                entityMessages.EntitySavedWithSuccessMessage(_departmentEntity.DepartmentName);
                return _department;
            }

            return entity;
        }

        private DepartmentModel SetDepartmentModel(DEPARTMENT _entity)
        {
            departmentModel.ID = _entity.ID;
            departmentModel.DepartmentName = _entity.DepartmentName;

            return departmentModel;
        }

        public IEnumerable<object> GetAllService()
        {
            var departments = _departmentRepository.GetAllEntitiesRepository();
            IEnumerable<DepartmentModel> departmentConverted = departments.Select(depart => DepartmentModel.FromDepartmentEntity(depart));
            return departmentConverted;
        }

        public List<DepartmentModel> GetAllModelService()
        {
            var departments = _departmentRepository.GetAllDepartmentEntities();
            List<DepartmentModel> departmentConverted = new List<DepartmentModel>();

            foreach (var item in departments)
            {
                var models = DepartmentModel.FromDepartmentEntity(item);
                departmentConverted.Add(models);
            }

            return departmentConverted;
        }

        public object GetEntityByIdService(object id)
        {
            var entity = _departmentRepository.GetEntityByIdRepository(id);
            return entity;
        }

        public void RemoveEntityService(object entity)
        {
            var _entity = SetDepartmentEntity(entity);

            _departmentRepository.RemoveEntityRepository(_entity);

            entityMessages.EntityDeletedWithSuccessMessage(_department.DepartmentName);
        }

        private DEPARTMENT SetDepartmentEntity(object entity)
        {
            var _entity = entity as DepartmentModel;
            _department.ID = _entity.ID;
            _department.DepartmentName = _entity.DepartmentName;
            return _department;
        }

        public object UpdateEntityService(object entity)
        {
            var departmentEntity = SetDepartmentEntity(entity);

            _departmentRepository.UpdateEntityRepository(departmentEntity);
            entityMessages.EntityUpdatedMessage(departmentEntity.DepartmentName);
            return entity as DepartmentModel;
        }
    }
}
