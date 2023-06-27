namespace DAL.DTO
{
    public class DepartmentDTO
    {
        public DepartmentDTO() { }


        public DepartmentDTO(int departmentId, string departmentName)
        {
            ID = departmentId;
            DepartmentName = departmentName;
        }

        public int ID { get; set; }
        public string DepartmentName { get; set; }
    }
}