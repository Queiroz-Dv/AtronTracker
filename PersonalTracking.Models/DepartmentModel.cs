using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalTracking.Models
{
    public class DepartmentModel
    {
        public DepartmentModel()
        {
            //Positions = new List<PositionModel>();
        }

        public int DepartmentModelId { get; set; }

        [Required(ErrorMessage = "The department field is required")]
        [MinLength(3)]
        [MaxLength(50)]
        public string DepartmentModelName { get; set; }

        public virtual ICollection<PersonModel> Staff { get; set; }
        //public IList<PositionModel> Positions { get; set; }
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


    // Object Model to test
    public class CompanyModel
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public virtual ICollection<DepartmentModel> Departments { get; set; }
    }

    [Table("Staff")]
    public class PersonModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Salary { get; set; }
    }

    public class  ProjectManagerModel : PersonModel
    {
        public string ProjectManagerProperty { get; set; }
    }

    public class DeveloperModel : PersonModel
    {
        public string DveeloperProperty { get; set; }
    }

    public class TesterModel : PersonModel
    {
        public string TesterProperty { get; set; }
    }
}
