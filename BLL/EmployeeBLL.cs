using DAL;
using DAL.DAO;
using DAL.DTO;
using DAL.Interfaces;
using PersonalTracking.Entities;
using PersonalTracking.Helper.Interfaces;
using PersonalTracking.Models;
using System.Collections.Generic;

namespace BLL
{
    public class EmployeeBLL
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IDepartmentRepository _deparmentRepository;
        private readonly IPositionRepository _positionRepository;

        private readonly EMPLOYEE employee;

        public EmployeeBLL(IEmployeeRepository employeeRepository,
                           IDepartmentRepository departmentRepository,
                           IPositionRepository positionRepository)
        {
            employee = new EMPLOYEE();
            _employeeRepository = employeeRepository;
            _deparmentRepository = departmentRepository;
            _positionRepository = positionRepository;
        }

        public EMPLOYEE GetEntityByIdService(object id)
        {
            throw new System.NotImplementedException();
        }

        public EMPLOYEE CreateEntityService(EMPLOYEE entity)
        {
            if (entity != null)
            {
                employee.ID = entity.ID;
                employee.UserNo = entity.UserNo;
                employee.Name = entity.Name;
                employee.Surname = entity.Surname;
                employee.DepartmentID = entity.DepartmentID;
                employee.PositionID = entity.PositionID;
                employee.Salary = entity.Salary;
                employee.BirthDay = entity.BirthDay;
                employee.Adress = entity.Adress;
                employee.Password = entity.Password;
                employee.isAdmin = entity.isAdmin;

                _employeeRepository.CreateEntityRepository(employee);
                return entity;
            }
            else
            {

                return employee;
            }
        }

        public EMPLOYEE UpdateEntityService(EMPLOYEE entity)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveEntityService(EMPLOYEE entity)
        {
            throw new System.NotImplementedException();
        }

        public static void UpdateEmployee(EMPLOYEE employee)
        {
            EmployeeDAO.UpdateEmployee(employee);
        }

        public static void DeleteEmployee(int employeeID)
        {
            EmployeeDAO.DeleteEmployee(employeeID);
        }

        public IEnumerable<EMPLOYEE> GetAllService()
        {
            var entities = _employeeRepository.GetAllEntitiesRepository();
            // TODO: verificar depois 
            return entities;
        }

        public static EmployeeDTO GetAll()
        {
            EmployeeDTO dto = new EmployeeDTO();
            //dto.Departments = DepartmentDAO.GetDepartments();
            dto.Positions = PositionDAO.GetPositions();
            dto.Employees = EmployeeDAO.GetEmployees();
            return dto;
        }

        public static void AddEmployee(EMPLOYEE employee)
        {
            EmployeeDAO.AddEmployee(employee);
        }

        public static bool isUnique(int v)
        {
            List<EMPLOYEE> list = EmployeeDAO.GetUsers(v);
            if (list.Count > 0)
                return false;
            else
                return true;
        }

        public static List<EMPLOYEE> GetEmployees(int v, string text)
        {
            return EmployeeDAO.GetEmployees(v, text);
        }

        public bool IsUniqueEntity(int entity)
        {
            List<EMPLOYEE> list = _employeeRepository.GetUsers(entity) as List<EMPLOYEE>;
            return list.Count <= 0;
        }
    }
}
