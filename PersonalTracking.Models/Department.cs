using System.Collections.Generic;

namespace PersonalTracking.Models
{
    public class Department
    {
        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; }

        public int PositionId { get; set; }

        public Position Position { get; set; }
    }
}
