using BLL.Services;
using DAL;
using DAL.DTO;
using DAL.Repositories;
using System.Collections.Generic;

namespace BLL
{
    public class DepartmentBLL : IDepartmentServices
    {
        private readonly DeparmentRepository _repository = new DeparmentRepository();
        private readonly DEPARTMENT entity = new DEPARTMENT();

        public DepartmentDTO CreateDepartmentServices(DepartmentDTO department)
        {
            entity.ID = department.ID;
            entity.DepartmentName = department.DepartmentName;
            _repository.CreateEntityRepository(entity);
            return department;
        }

        public void DeleteAllDepartmentsService(ICollection<DepartmentDTO> department)
        {
            throw new System.NotImplementedException();
        }

        public DepartmentDTO DeleteDepartmentByIdService(DepartmentDTO department)
        {
            throw new System.NotImplementedException();
        }

        public ICollection<DepartmentDTO> GetAllDepartmentsService()
        {
            throw new System.NotImplementedException();
        }

        public DepartmentDTO GetDepartmentByIdService(int? id)
        {
            var department = _repository.GetEntityByIdRepository((int)id);
            return department as DepartmentDTO;
        }

        public DepartmentDTO UpdateDepartmentService(DepartmentDTO department)
        {
            throw new System.NotImplementedException();
        }
    }
}
