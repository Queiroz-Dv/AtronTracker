using BLL.Services;
using DAL;
using DAL.DAO;
using DAL.DTO;
using DAL.Generics;
using DAL.Interfaces;
using System.Collections.Generic;

namespace BLL
{
    public class DepartmentBLL : IDepartmentServices
    {
        private readonly Context _context;
        private readonly IDeparmentRepository _repository;

        public DepartmentBLL(Context context, IGenericRepository<DEPARTMENT> repository)
        {
            _context = context;
            _repository = repository as IDeparmentRepository;
        }

        public DepartmentDTO CreateDepartmentServices(DepartmentDTO department)
        {
            var departmentEntity = _repository.GetEntityByIdRepository(department.Id);

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
            throw new System.NotImplementedException();
        }

        public DepartmentDTO UpdateDepartmentService(DepartmentDTO department)
        {
            throw new System.NotImplementedException();
        }
    }
}
