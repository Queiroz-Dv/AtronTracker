using Models.ValueObjects;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    [Table("Employees")]
    public class Employee
    {
        public int Id { get; set; }
        public long UserNo { get; set; }
        public FullName FullName { get; set; }
        public Department Department { get; set; }
        public Position Position { get; set; }
        public Salary Salary { get; set; }
        public DateTime BirthDay { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
    }
}
