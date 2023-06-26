using PersonalTracking.DAL.DataAcess;
using System.Collections.Generic;

namespace PersonalTracking.DAL.DTO
{
    public class TaskDTO
    {
        public List<EmployeeDetailDTO> Employees { get; set; }

        public List<DEPARTMENT> Departments { get; set; }

        public List<PositionDTO> Positions { get; set; }

        public List<TASKSTATE> TaskStates { get; set; }

        public List<TaskDetailDTO> Tasks { get; set; }
    }
}
