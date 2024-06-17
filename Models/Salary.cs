namespace Models
{
    public class Salary
    {
        public int Id { get; set; }
        public Employee Employee { get; set; }
        public int Amount { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
    }
}