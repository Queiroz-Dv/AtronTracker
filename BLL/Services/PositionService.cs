using BLL.Interfaces;
using DAL.Interfaces;
using PersonalTracking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    /// <summary>
    /// Service do módulo de Cargos
    /// </summary>
    public class PositionService : IPositionService
    {
        private readonly IPositionRepository _positionRepository;
        private readonly PositionModel positionModel;

        public PositionService(IPositionRepository positionRepository)
        {
            _positionRepository = positionRepository;
            positionModel = new PositionModel();
        }

        public PositionModel CreateEntityService(PositionModel entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PositionModel> GetAllService()
        {
            var positions = _positionRepository.GetAllEntitiesRepository().ToList();
            return positions;
        }

        public PositionModel GetEntityByIdService(object id)
        {
            throw new NotImplementedException();
        }

        public PositionModel RemoveEntityService(object entity)
        {
            throw new NotImplementedException();
        }

        public PositionModel UpdateEntityService(PositionModel entity)
        {
            throw new NotImplementedException();
        }
    }
}
