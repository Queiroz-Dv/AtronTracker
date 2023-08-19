using DAL.DTO;
using PersonalTracking.Models;

namespace DAL.Interfaces.FactoryModules
{
    public interface IPositionFactory
    {
        PositionDTO CreateModelToDalEntity(PositionModel positionModel);

        PositionModel CreateDalToModel(PositionModel position);

        PositionModel CreateDalToModel(PositionDTO position);

        PositionModel SetPositionModelFactory(PositionModel positionModel);
    }
}