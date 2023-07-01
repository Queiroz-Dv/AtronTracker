using HLP.Entity;
using HLP.Interfaces;

namespace PersonalTracking.Models
{
    public class DepartmentModel : ValidateModel
    {
        private readonly IEntityMessages messageHelper;

        public int DepartmentModelId { get; set; }

        public string DepartmentModelName { get; set; }

        public DepartmentModel(IEntityMessages messageHelper)
        {
            this.messageHelper = messageHelper;
        }

        public DepartmentModel()
        {

        }

        public override void Validate()
        {
            Errors.Clear();

            var departmentNameIsEmpty = FieldValidate(string.IsNullOrEmpty(DepartmentModelName));
            var departmentNameCharactersIsValid = FieldValidate(DepartmentModelName.Length < 3);

            if (departmentNameIsEmpty)
            {
                var errorMessage = messageHelper.FieldIsEmptyMessage(nameof(DepartmentModelName));

                Errors.Add(errorMessage);
            }

            if (departmentNameCharactersIsValid)
            {
                var errorMessage = messageHelper.InvalidMinimumAmountCharactersMessage(nameof(DepartmentModelName));
                Errors.Add(errorMessage);
            }
        }

        public void ShowMessageBoxErrors()
        {
            ShowMessageBoxUI();
        }
    }
}
