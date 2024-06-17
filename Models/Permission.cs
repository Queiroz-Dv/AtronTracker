using System;

namespace Models
{
    public class Permission
    {
        public int Id { get; set; }
        public int Employee { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PermissionState { get; set; }
        public string Explanation { get; set; }
        public int PermissionDay { get; set; }
    }
}