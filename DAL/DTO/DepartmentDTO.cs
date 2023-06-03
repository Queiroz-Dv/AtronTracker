using DAL.Generics;
using NTF.Entities;

namespace DAL.DTO
{
    public class DepartmentDTO : EntityModel, IEntity
    {
        protected DepartmentDTO() { }

        public DepartmentDTO(int departmentId, string departmentName)
        {
            Id = departmentId;
            DepartmentName = departmentName;
            Validate();
        }

        public sealed override void Validate()
        {
            IsInvalidId(Id, InvalidId);
            IsInvalidName(DepartmentName, InvalidName);
        }

        public int Id { get; set; }

        public string DepartmentName { get; set; }

        public PositionDTO CargoDTO { get; set; }
    }
}
