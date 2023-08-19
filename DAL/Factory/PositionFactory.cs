using DAL.DTO;
using DAL.Interfaces.FactoryModules;
using PersonalTracking.Models;

namespace DAL.Factory
{
    public class PositionFactory : IPositionFactory
    {
        protected PositionModel positionModel;
        protected PositionDTO positionDTO;

        public PositionFactory()
        {
            positionModel = new PositionModel();
            positionDTO = new PositionDTO();
        }

        public PositionModel CreateDalToModel(PositionModel position)
        {
            if (!position.Equals(null))
            {
                positionModel.PositionId = position.PositionId;
                positionModel.PositionName = position.PositionName;
                positionModel.Department.DepartmentModelId = position.Department.DepartmentModelId;
                positionModel.Department.DepartmentModelName = position.Department.DepartmentModelName;
                return positionModel;
            }
            else
            {
                return null;
            }
        }

        public PositionModel CreateDalToModel(PositionDTO position)
        {
            if (!position.Equals(null))
            {
                positionModel.PositionId = position.ID;
                positionModel.PositionName = position.PositionName;
                positionModel.Department.DepartmentModelId = position.DepartmentID;
                positionModel.Department.DepartmentModelName = position.DepartmentName;
                return positionModel;
            }
            else
            {
                return null;
            }
        }

        public PositionDTO CreateModelToDalEntity(PositionModel position)
        {
            if (!position.Equals(null))
            {
                positionDTO.ID = position.PositionId;
                positionDTO.PositionName = position.PositionName;
                positionDTO.DepartmentID = position.Department.DepartmentModelId;
                positionDTO.DepartmentName = position.Department.DepartmentModelName;

                return positionDTO;
            }
            else
            {
                return null;
            }
        }

        public PositionModel SetPositionModelFactory(PositionModel position)
        {
            if (!position.Equals(null))
            {
                positionModel = position;
                return positionModel;
            }

            return null;
        }
    }
}