using PersonalTracking.DAL.DataAcess;
using System.Collections.Generic;

namespace PersonalTracking.DAL.DTO
{
    public class SalaryDTO
    {
        public List<SalaryDetailDTO> Salaries { get; set; }

        public List<EmployeeDetailDTO> Employees { get; set; }

        public List<MONTH> Months { get; set; }

        public List<DEPARTMENT> Departments { get; set; }

        public List<PositionDTO> Positions { get; set; }
    }
}
