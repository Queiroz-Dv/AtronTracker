using BLL.Interfaces;
using DAL;
using DAL.Interfaces;
using System.Collections.Generic;

namespace BLL.Services
{
    public class DepartmentService : IDepartmentService<DEPARTMENT>
    {
        private readonly IDepartmentRepository<DEPARTMENT> _departmentRepository;
        private readonly DEPARTMENT _department;

        public DepartmentService(IDepartmentRepository<DEPARTMENT> departmentRepository)
        {
            _departmentRepository = departmentRepository;
            _department = new DEPARTMENT();
        }

        public DEPARTMENT CreateEntityService(DEPARTMENT entity)
        {
            _department.ID = entity.ID;
            _department.DepartmentName = entity.DepartmentName;
            _departmentRepository.CreateEntityRepository(entity);
            return _department;
        }

        public IEnumerable<DEPARTMENT> GetAllService()
        {
            var departments = _departmentRepository.GetAllEntitiesRepository();
            return departments;
        }

        public DEPARTMENT GetEntityByIdService(object id)
        {
            var entity = _departmentRepository.GetEntityByIdRepository(id);
            return entity;
        }

        public void RemoveEntityService(DEPARTMENT entity)
        {
            _departmentRepository.RemoveEntityRepository(entity);
        }

        public DEPARTMENT UpdateEntityService(DEPARTMENT entity)
        {
            _department.DepartmentName = entity.DepartmentName;
            _departmentRepository.UpdateEntityRepository(entity);
            return entity;
        }
    }
}
