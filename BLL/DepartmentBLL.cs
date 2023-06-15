using BLL.Interfaces;
using DAL;
using DAL.Repositories;
using System.Collections.Generic;

namespace BLL
{
    public class DepartmentBLL : IGenericBLL<DEPARTMENT>
    {
        private readonly DeparmentRepository _repository = new DeparmentRepository();
        private readonly DEPARTMENT department = new DEPARTMENT();

        public DEPARTMENT CreateEntityBLL(DEPARTMENT entity)
        {
            department.ID = entity.ID;
            department.DepartmentName = entity.DepartmentName;
            _repository.CreateEntity(entity);
            return department;
        }

        public IEnumerable<DEPARTMENT> GetAllEntitiesBLL()
        {
            var departments = _repository.GetAllEntities();
            return departments;
        }

        public DEPARTMENT GetEntityByIdBLL(object id)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveEntityBLL(DEPARTMENT entity)
        {
            _repository.RemoveEntity(entity);
        }

        public DEPARTMENT UpdateEntityBLL(DEPARTMENT entity)
        {
            department.DepartmentName = entity.DepartmentName;
            _repository.UpdateEntity(entity);
            return entity;
        }
    }
}