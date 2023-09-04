using DAL.DTO;
using DAL.Factory;
using DAL.Interfaces;
using PersonalTracking.Entities;
using PersonalTracking.Factory.Interfaces;
using PersonalTracking.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAL.Repositories
{
    public class PositionRepository : ContextBase<PositionModel, POSITION>, IPositionRepository
    {
        public PositionRepository(IModelFactory<PositionModel, POSITION> modelFactory)
            : base(modelFactory)
        {
            _factory = modelFactory;
        }

        public PositionModel GetEntityByIdRepository(object id)
        {
            try
            {
                var position = GetContext().POSITIONs.FirstOrDefault(pos => pos.ID.Equals(id));

                var positionDTO = position as PositionDTO;

                var positionModel = _factory.ToModel(positionDTO);

                return positionModel;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void CreateEntityRepository(PositionModel positionModel)
        {
            try
            {
                var positionDTO = _factory.ToEntity(positionModel);

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

                var positionModel = _factory.ToModel(positionDTO);

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

                var positionModel = _factory.ToModel(positionDTO);
                return positionModel;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
