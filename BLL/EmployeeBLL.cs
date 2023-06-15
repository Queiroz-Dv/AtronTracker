using BLL.Interfaces;
using DAL;
using DAL.DAO;
using DAL.DTO;
using DAL.Repositories;
using System.Collections.Generic;

namespace BLL
{
    public class EmployeeBLL : IEmployeeBLL<EMPLOYEE>
    {
        private readonly EmployeeRepository repository = new EmployeeRepository();
        private readonly EMPLOYEE employee = new EMPLOYEE();

        public EMPLOYEE GetEntityByIdBLL(object id)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<EMPLOYEE> GetAllEntitiesBLL()
        {
            throw new System.NotImplementedException();
        }

        public EMPLOYEE CreateEntityBLL(EMPLOYEE entity)
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

            repository.CreateEntity(employee);
            return employee;
        }

        public EMPLOYEE UpdateEntityBLL(EMPLOYEE entity)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveEntityBLL(EMPLOYEE entity)
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

        public bool isUniqueEntity(int entity)
        {
            List<EMPLOYEE> list = repository.GetUsers(entity) as List<EMPLOYEE>;
            return list.Count <= 0;
        }
    }
}
