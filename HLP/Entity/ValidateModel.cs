using HLP.Interfaces;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HLP.Entity
{
    public abstract class ValidateModel : InformationMessage, IValidateModel, IValidateHelper, IShowMessagesUI
    {
        public IList<object> Errors { get; private set; }

        public bool IsValidModel
        {
            get
            {
                return Errors.Count == 0;
            }
        }

        protected ValidateModel()
        {
            Errors = new List<object>();
        }

        public abstract void Validate();

        public bool FieldValidate(bool condition)
        {
            return condition;
        }

        public void ShowMessageBoxUI()
        {
            foreach (string error in Errors)
            {
                MessageBox.Show(error, "Validation Errors", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

}
