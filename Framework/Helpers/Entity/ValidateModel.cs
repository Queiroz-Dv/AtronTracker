using Helpers.Interfaces;
using Notification.Interfaces;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Helpers.Entity
{
    public abstract class ValidateModel<T> : InformationMessage, IValidateModel, IValidateHelper<T>, IShowMessagesUI
    {
        public IList<object> Errors { get; private set; }

        public bool IsValidModel
        {
            get
            {
                return Errors.Count == 0;
            }
        }

        public INotificationService NotificationService { get; set; }

        protected ValidateModel()
        {
            Errors = new List<object>();
        }

        public abstract void Validate();

        public void Validate(T condition) { }

        public void ShowMessageBoxUI()
        {
            foreach (string error in Errors)
            {
                MessageBox.Show(error, "Validation Errors", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}