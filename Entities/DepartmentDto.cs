using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class DepartmentDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The department field is required")]
        [MinLength(3, ErrorMessage = "The department must be 3 or 50 characters")]
        [MaxLength(50, ErrorMessage = "The department must be 3 or 50 characters")]
        public string Name { get; set; }
    }
}