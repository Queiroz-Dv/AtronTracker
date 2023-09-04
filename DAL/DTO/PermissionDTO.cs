using PersonalTracking.Entities;
using System.Collections.Generic;

namespace DAL.DTO
{
    public class PermissionDTO
    {
        public List<DEPARTMENT> Departments { get; set; }

        public List<PositionDTO> Positions { get; set; }

        public List<PERMISSIONSTATE> States { get; set; }

        public List<PermissionDetailDTO> Permissions { get; set; }
    }
}
