using System.Collections.Generic;

namespace PersonalTracking.Models
{
    public class Position
    {
        public int PositionId { get; set; }

        public string PositionName { get; set; }

        public int OldDepartmentId { get; set; }

        public ICollection<Department> Departments { get; set; }
    }
}
