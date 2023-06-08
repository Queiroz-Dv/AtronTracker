using System;

namespace DAL.DTO
{
    public class DepartmentDTO
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

        public int Id { get; set; }

        public string DepartmentName { get; set; }
    }
}
