using BLL.Interfaces;
using DAL;
using DAL.DAO;
using DAL.DTO;
using DAL.Interfaces;
using DAL.Repositories;
using System.Collections.Generic;

namespace BLL
{
    public class EmployeeBLL 
    {
        private readonly IEmployeeRepository<EMPLOYEE> repository;
        private readonly DepartmentRepository deparmentRepository;
        private readonly PositionRepository positionRepository;

        private readonly EMPLOYEE employee;

        public EmployeeBLL()
        {
            
            employee = new EMPLOYEE();
            deparmentRepository = new DepartmentRepository();
            positionRepository = new PositionRepository();
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
                employee.Address = entity.Address;
                employee.Password = entity.Password;
                employee.isAdmin = entity.isAdmin;

                repository.CreateEntityRepository(employee);
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
            var entities = repository.GetAllEntitiesRepository();
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
            List<EMPLOYEE> list = repository.GetUsers(entity) as List<EMPLOYEE>;
            return list.Count <= 0;
        }
    }
}
