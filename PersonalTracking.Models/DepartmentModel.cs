using System.ComponentModel.DataAnnotations;

namespace PersonalTracking.Models
{
    public class DepartmentModel
    {
        public int DepartmentModelId { get; set; }

        [Required(ErrorMessage = "The department field is required")]
        [MinLength(3)]
        [MaxLength(50)]
        public string DepartmentModelName { get; set; }
    }

    public class PositionModel
    {
        public int PositionId { get; set; }

        public string PositionName { get; set; }

        public int OldDepartmentID { get; set; }

        public DepartmentModel DepartmentName { get; set; }
    }
}
