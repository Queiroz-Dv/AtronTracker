using PersonalTracking.Helpers.Entity;
using PersonalTracking.Helpers.Interfaces;

namespace PersonalTracking.Helpers.EntityUtils
{
    public abstract class ValidateHelper : IValidateHelper
    {
        public bool FieldValidate(bool condition)
        {
            return condition;
        }

        public static InformationMessage ValidateErrorMessage(IEntityMessages message)
        {
            return (InformationMessage)message;
        }
    }
}
