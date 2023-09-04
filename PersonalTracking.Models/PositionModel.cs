using System.Collections.Generic;

namespace PersonalTracking.Models
{
    public class PositionModel
    {
        public int PositionId { get; set; }

        public string PositionName { get; set; }

        public int OldDepartmentID { get; set; }

        public IList<DepartmentModel> Department { get; set; }
    }
}