using BLL.Interfaces;
using DAL.Interfaces;
using PersonalTracking.Models;
using System.Collections.Generic;

namespace BLL.Services
{
    /// <summary>
    /// Service do módulo de Cargos
    /// </summary>
    public class PositionService : IPositionService
    {
        private readonly IPositionRepository _positionRepository;
        private readonly IEmployeeRepository _employeeRepository;
        //private readonly IPositionFactory _positionFactory;

        public PositionService(IPositionRepository positionRepository, IEmployeeRepository employeeRepository)
        {
            _positionRepository = positionRepository;
            _employeeRepository = employeeRepository;
            //_positionFactory = positionFactory;
        }

        public PositionModel CreateEntityService(PositionModel entity)
        {
            return null;
            //var position = _positionFactory.SetPositionModelFactory(entity);
            //if (!position.Equals(null))
            //{
            //    _positionRepository.CreateEntityRepository(position);
            //    return position;
            //}
            //else
            //{
            //    return entity;
            //}
        }

        public IEnumerable<PositionModel> GetAllService()
        {
            var positions = _positionRepository.GetAllEntitiesRepository();
            return positions;
        }

        public PositionModel GetEntityByIdService(object id)
        {
            var position = _positionRepository.GetEntityByIdRepository(id);
            return position;
        }

        public PositionModel RemoveEntityService(object entity)
        {
            var position = _positionRepository.RemoveEntityRepository(entity);
            return position;
        }

        public bool control;

        public PositionModel UpdateEntityService(PositionModel entity)
        {

            var position = _positionRepository.UpdateEntityRepository(entity);
            if (control)
            {
                _employeeRepository.UpdateEntityRepository(position);
            }
            return position;
        }
    }
}
