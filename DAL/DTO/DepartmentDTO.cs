using System;

namespace DAL.DTO
{
    public class DepartmentDTO : DEPARTMENT
    {
        public DepartmentDTO() { }


        public DepartmentDTO(int departmentId, string departmentName)
        {
            Id = departmentId;
            DepartmentName = departmentName;
            Validate();
        }

        private void Validate()
        {
            throw new NotImplementedException();
        }
    }
}