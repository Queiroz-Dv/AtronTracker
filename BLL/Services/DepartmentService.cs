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
        private DepartmentModel departmentModel;
        private List<DepartmentModel> departmentModels;

        public DepartmentService(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
            entityMessages = new InformationMessage();
            departmentModel = new DepartmentModel(entityMessages);
            departmentModels = new List<DepartmentModel>();
        }

        public DepartmentModel CreateEntityService(DepartmentModel entity)
        {
            var departmentModel = SetDepartmentModel(entity);

            departmentModel.Validate();
            if (!departmentModel.IsValidModel)
            {
                foreach (DepartmentModel item in departmentModel.Errors)
                {
                    item.ShowMessageBoxErrors();
                }
            }
            else
            {
                _departmentRepository.CreateEntityRepository(departmentModel);
                entityMessages.EntitySavedWithSuccessMessage(departmentModel.DepartmentModelName);
                return departmentModel;
            }

            return departmentModel;
        }

        private DepartmentModel SetDepartmentModel(DepartmentModel _entity)
        {
            departmentModel.DepartmentModelId = _entity.DepartmentModelId;
            departmentModel.DepartmentModelName = _entity.DepartmentModelName;

            return departmentModel;
        }

        private DepartmentModel ConvertDepartmentDalToDepartmentModel(object _entity)
        {
            departmentModel.DepartmentModelId = _entity.ID;
            departmentModel.DepartmentModelName = _entity.DepartmentName;

            return departmentModel;
        }

        public IEnumerable<DepartmentModel> GetAllService()
        {
            var departments = _departmentRepository.GetAllEntitiesRepository();
            List<DepartmentModel> departmentConverted = ConvertObjectHelper.ConvertList(departments, departmentModels);
            return departmentConverted;
        }

        public List<DepartmentModel> GetAllModelService()
        {
            var departments = _departmentRepository.GetAllDepartmentEntities();

            var departmentsConverted = ConvertObjectHelper.ConvertList(departments, entitiesConverted => ConvertDepartmentDalToDepartmentModel(entitiesConverted));

            if (departmentsConverted is List<DepartmentModel>)
            {

            }
            //var departments = _departmentRepository.GetAllDepartmentEntities();
            //List<DepartmentModel> departmentConverted = new List<DepartmentModel>();

            //foreach (var item in departments)
            //{
            //    var models = DepartmentModel.FromDepartmentEntity(item);
            //    departmentConverted.Add(models);
            //}

            //return departmentConverted;
        }

        public DepartmentModel GetEntityByIdService(object id)
        {
            var entity = _departmentRepository.GetEntityByIdRepository(id);
            return entity;
        }

        public void RemoveEntityService(DepartmentModel model)
        {
            var _entity = SetDepartmentModel(model);

            _departmentRepository.RemoveEntityRepository(_entity);

            entityMessages.EntityDeletedWithSuccessMessage(_entity.DepartmentModelName);
        }

        public DepartmentModel UpdateEntityService(DepartmentModel entity)
        {
            var departmentEntity = SetDepartmentModel(entity);

            _departmentRepository.UpdateEntityRepository(departmentEntity);
            entityMessages.EntityUpdatedMessage(departmentEntity.DepartmentModelName);
            return entity;
        }
    }
}
