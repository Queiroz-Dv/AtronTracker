using System;

namespace Models
{
    public class Task
    {
        public int Id { get; set; }
        public int Employee { get; set; }
        public string TaskTitle { get; set; }
        public string TaskContent { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public int TaskState { get; set; }
    }
}