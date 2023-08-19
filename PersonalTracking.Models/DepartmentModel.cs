using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PersonalTracking.Models
{
    public class DepartmentModel
    {
        public DepartmentModel()
        {
            Positions = new List<PositionModel>();
        }

        public int DepartmentModelId { get; set; }

        [Required(ErrorMessage = "The department field is required")]
        [MinLength(3)]
        [MaxLength(50)]
        public string DepartmentModelName { get; set; }

        public IList<PositionModel> Positions { get; set; }
    }

    public class PositionModel 
    {
        public PositionModel()
        {
            Department = new DepartmentModel();
        }

        public int PositionId { get; set; }

        public string PositionName { get; set; }

        public int OldDepartmentID { get; set; }

        public DepartmentModel Department { get; set; } 
    }

    public class EmployeeModel
    {

    }
}
