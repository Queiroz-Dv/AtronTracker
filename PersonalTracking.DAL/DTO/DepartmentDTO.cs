﻿using PersonalTracking.DAL.DataAcess;

namespace PersonalTracking.DAL.DTO
{
    public class DepartmentDTO : DEPARTMENT
    {
        public DepartmentDTO() { }


        public DepartmentDTO(int departmentId, string departmentName)
        {
            ID = departmentId;
            DepartmentName = departmentName;
        }
    }
}