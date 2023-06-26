using BLL.Interfaces;
using DAL;
using DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL.Services
{
    public class EmployeeService : IEmployeeService<EMPLOYEE>
    {
        // Inversão de controle && Injeção de dependência
        private readonly IEmployeeRepository<EMPLOYEE> _employeeRepository;
        private readonly IDepartmentRepository _deparmentRepository;
        private readonly IPositionRepository<POSITION> _positionRepository;
        private readonly EMPLOYEE _employee;

        public EmployeeService(IEmployeeRepository<EMPLOYEE> employeeRepository,
                               IDepartmentRepository departmentRepository,
                               IPositionRepository<POSITION> positionRepository)
        {
            _employeeRepository = employeeRepository;
            _deparmentRepository = departmentRepository;
            _positionRepository = positionRepository;
            _employee = new EMPLOYEE();
        }

        public EMPLOYEE CreateEntityService(EMPLOYEE entity)
        {
            if (entity != null)
            {
                _employee.ID = entity.ID;
                _employee.UserNo = entity.UserNo;
                _employee.Name = entity.Name;
                _employee.Surname = entity.Surname;
                _employee.DepartmentID = entity.DepartmentID;
                _employee.PositionID = entity.PositionID;
                _employee.Salary = entity.Salary;
                _employee.BirthDay = entity.BirthDay;
                _employee.Address = entity.Address;
                _employee.Password = entity.Password;
                _employee.isAdmin = entity.isAdmin;

                _employeeRepository.CreateEntityRepository(_employee);
                return entity;
            }
            else
            {
                return _employee;
            }
        }

        public IEnumerable<EMPLOYEE> GetAllService()
        {
            var entities = _employeeRepository.GetAllEntitiesRepository();
            return entities;
        }

        public IList<EMPLOYEE> GetEmployeesByUserNoAndPasswordService(int userNumber, string password)
        {

            var entities = _employeeRepository.GetEmployeesByUserNoAndPassword(userNumber, password).ToList();
            return entities;
        }

        public EMPLOYEE GetEntityByIdService(object id)
        {
            var entity = _employeeRepository.GetEntityByIdRepository(id);
            return entity;
        }

        public bool IsUniqueEntity(int entity)
        {
            IList<EMPLOYEE> list = _employeeRepository.GetUsers(entity) as List<EMPLOYEE>;
            return list.Count <= 0;
        }

        public void RemoveEntityService(EMPLOYEE entity)
        {
            throw new NotImplementedException();
        }

        public EMPLOYEE UpdateEntityService(EMPLOYEE entity)
        {
            throw new NotImplementedException();
        }
    }
}