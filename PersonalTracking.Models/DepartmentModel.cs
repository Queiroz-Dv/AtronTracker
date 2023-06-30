using DAL;
using HLP.Entity;
using HLP.Interfaces;

namespace PersonalTracking.Models
{
    public class DepartmentModel : ValidateModel
    {
        private readonly IEntityMessages messageHelper;

        public int ID { get; set; }

        public string DepartmentName { get; set; }

        public DepartmentModel(IEntityMessages messageHelper)
        {
            this.messageHelper = messageHelper;
        }

        protected DepartmentModel()
        {

        }

        public override void Validate()
        {
            Errors.Clear();

            var departmentNameIsEmpty = FieldValidate(string.IsNullOrEmpty(DepartmentName));
            var departmentNameCharactersIsValid = FieldValidate(DepartmentName.Length < 3);

            if (departmentNameIsEmpty)
            {
                var errorMessage = messageHelper.FieldIsEmptyMessage(nameof(DepartmentName));

                Errors.Add(errorMessage);
            }

            if (departmentNameCharactersIsValid)
            {
                var errorMessage = messageHelper.InvalidMinimumAmountCharactersMessage(nameof(DepartmentName));
                Errors.Add(errorMessage);
            }
        }

        public void ShowMessageBoxErrors()
        {
            ShowMessageBoxUI();
        }

        public static DepartmentModel FromDepartmentEntity(DEPARTMENT departmentDAL)
        {
            DepartmentModel departmentModel = new DepartmentModel
            {
                ID = departmentDAL.ID,
                DepartmentName = departmentDAL.DepartmentName
            };
            return departmentModel;
        }
    }
}
