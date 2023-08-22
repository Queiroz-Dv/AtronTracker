using DAL.DTO;
using DAL.Factory;
using DAL.Interfaces;
using DAL.Interfaces.FactoryModules;
using PersonalTracking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using PersonalTracking.Helper.Interfaces;

namespace DAL.Repositories
{
    public class PositionRepository : ContextBase<PositionModel, PositionDTO>, IPositionRepository
    {
        private readonly IPositionFactory _positionFactory;

        public PositionRepository(IPositionFactory positionFactory, IObjectModelHelper<PositionModel, PositionDTO> objectModelHelper)
            : base(objectModelHelper)
        {
            _positionFactory = positionFactory;
        }

        public PositionModel GetEntityByIdRepository(object id)
        {
            try
            {
                var position = GetContext().POSITIONs.FirstOrDefault(pos => pos.ID.Equals(id));

                var positionDTO = position as PositionDTO;

                var positionModel = _objectModelHelper.CreateModel(positionDTO, positionFunction => _positionFactory.CreateDalToModel(positionDTO));

                return positionModel;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void CreateEntityRepository(PositionModel positionModel)
        {
            try
            {
                var positionDTO = _objectModelHelper.CreateEntity(positionModel, positionFunc => _positionFactory.CreateModelToDalEntity(positionModel));

                var position = new POSITION();

                position.ID = positionDTO.ID;
                position.PositionName = positionDTO.PositionName;
                //position.DepartmentID = positionDTO.DepartmentID;

                GetContext().POSITIONs.InsertOnSubmit(position);
                GetContext().SubmitChanges();

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public IEnumerable<PositionModel> GetAllEntitiesRepository()
        {
            try
            {
                //var positionsWithDepartments = db.PositionWithDepartments.ToList();

                //IList<PositionModel> positionList = new List<PositionModel>();

                //foreach (var item in positionsWithDepartments)
                //{
                //    PositionModel positionModel = new PositionModel();

                //    positionModel.PositionId = item.Position_ID;
                //    positionModel.PositionName = item.Position_Name;
                //    positionModel.Department.DepartmentModelId = item.Department_ID;
                //    positionModel.Department.DepartmentModelName = item.Department_Name;

                //    positionList.Add(positionModel);
                //}

                //return positionList;
                return null;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public PositionModel RemoveEntityRepository(object entity)
        {
            try
            {
                var position = GetContext().POSITIONs.First(pos => pos.ID.Equals(entity));

                GetContext().POSITIONs.DeleteOnSubmit(position);
                GetContext().SubmitChanges();

                var positionDTO = position as PositionDTO;

                var positionModel = _objectModelHelper.CreateModel(positionDTO, positionFunction => _positionFactory.CreateDalToModel(positionDTO));

                return positionModel;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public PositionModel UpdateEntityRepository(PositionModel entity)
        {
            try
            {
                var position = GetContext().POSITIONs.First(x => x.ID.Equals(entity.PositionId));


                position.PositionName = entity.PositionName;
                //position.DepartmentID = entity.Department.DepartmentModelId;

                GetContext().SubmitChanges();

                var positionDTO = position as PositionDTO;

                var positionModel = _objectModelHelper.CreateModel(positionDTO, positionFunction => _positionFactory.CreateDalToModel(positionDTO));
                return positionModel;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
