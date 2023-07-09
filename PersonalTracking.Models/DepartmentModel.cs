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
}
