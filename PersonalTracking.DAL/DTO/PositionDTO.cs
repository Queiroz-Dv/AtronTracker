using PersonalTracking.DAL.DataAcess;

namespace PersonalTracking.DAL.DTO
{
    public class PositionDTO : POSITION
    {
        public string DepartmentName { get; set; }
        public int OldDepartmentID { get; set; }
    }
}
