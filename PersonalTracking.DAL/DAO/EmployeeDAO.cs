using PersonalTracking.DAL.DataAcess;
using PersonalTracking.DAL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PersonalTracking.DAL.DAO
{
    public class EmployeeDAO
    {
        public static PersonalTrackingDBDataContext GetContextEmployeDAO()
        {
            var context = Context.DB;
            return context;
        }

        public void AddEmployee(EMPLOYEE employee)
        {
            try
            {
                var context = GetContextEmployeDAO();
                context.EMPLOYEEs.InsertOnSubmit(employee);
                context.SubmitChanges();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public static List<EmployeeDetailDTO> GetEmployees()
        {
            var context = GetContextEmployeDAO();
            List<EmployeeDetailDTO> employeeList = new List<EmployeeDetailDTO>();
            var list = (from e in context.EMPLOYEEs
                        join d in context.DEPARTMENTs on e.DepartmentID equals d.ID
                        join p in context.POSITIONs on e.PositionID equals p.ID
                        select new
                        {
                            UserNo = e.UserNo,
                            Name = e.Name,
                            Surname = e.Surname,
                            EmployeeID = e.ID,
                            Password = e.Password,
                            DepartmentName = d.DepartmentName,
                            PositionName = p.PositionName,
                            DepartmentID = e.DepartmentID,
                            PositionId = e.PositionID,
                            isAdmin = e.isAdmin,
                            Salary = e.Salary,
                            Birthday = e.BirthDay,
                            Address = e.Adress
                        }).OrderBy(x => x.UserNo).ToList();

            foreach (var item in list)
            {
                EmployeeDetailDTO dto = new EmployeeDetailDTO();
                var departmentId = Convert.ToInt32(item.DepartmentID);
                var positionId = Convert.ToInt32(item.PositionId);
                var salary = Convert.ToInt32(item.Salary);

                dto.Name = item.Name;
                dto.UserNo = item.UserNo;
                dto.Surname = item.Surname;
                dto.EmployeeID = item.EmployeeID;
                dto.Password = item.Password;
                dto.DepartmentID = departmentId;
                dto.DepartmentName = item.DepartmentName;
                dto.PositionID = positionId;
                dto.PositionName = item.PositionName;
                dto.isAdmin = item.isAdmin;
                dto.Salary = salary;
                dto.BirthDay = item.Birthday;
                dto.Address = item.Address;
                employeeList.Add(dto);
            }

            return employeeList;
        }

        public static void DeleteEmployee(int employeeID)
        {
            var context = GetContextEmployeDAO();
            EMPLOYEE emp = context.EMPLOYEEs.First(x => x.ID == employeeID);
            context.EMPLOYEEs.DeleteOnSubmit(emp);
            context.SubmitChanges();

        }

        public static void UpdateEmployee(POSITION position)
        {
            var context = GetContextEmployeDAO();
            List<EMPLOYEE> list = context.EMPLOYEEs.Where(x => x.PositionID == position.ID).ToList();
            foreach (var item in list)
            {
                item.DepartmentID = position.DepartmentID;
            }
            context.SubmitChanges();
        }

        public static void UpdateEmployee(EMPLOYEE employee)
        {
            try
            {
                var context = GetContextEmployeDAO();
                EMPLOYEE emp = context.EMPLOYEEs.First(x => x.ID == employee.ID);
                emp.UserNo = employee.UserNo;
                emp.Name = employee.Name;
                emp.Surname = employee.Surname;
                emp.Password = employee.Password;
                emp.isAdmin = employee.isAdmin;
                emp.BirthDay = employee.BirthDay;
                emp.Adress = employee.Adress;
                emp.DepartmentID = employee.DepartmentID;
                emp.PositionID = employee.PositionID;
                emp.Salary = employee.Salary;
                context.SubmitChanges();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public static void UpdateEmployee(int employeeID, int amount)
        {
            try
            {
                var db = GetContextEmployeDAO();
                EMPLOYEE employee = db.EMPLOYEEs.First(x => x.ID == employeeID);
                employee.Salary = amount;
                db.SubmitChanges();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static List<EMPLOYEE> GetEmployees(int v, string text)
        {
            try
            {
                var db = GetContextEmployeDAO();
                List<EMPLOYEE> list = db.EMPLOYEEs.Where(x => x.UserNo == v && x.Password == text).ToList();
                return list;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static List<EMPLOYEE> GetUsers(int v)
        {
            var db = GetContextEmployeDAO();
            return db.EMPLOYEEs.Where(x => x.UserNo == v).ToList();
        }
    }
}