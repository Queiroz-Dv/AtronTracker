using HLP.Entity;
using HLP.Interfaces;

namespace HLP.Helpers
{
    public abstract class ValidateHelper : IValidateHelper
    {
        public bool Validate(bool condition)
        {
            return condition;
        }

        public InformationMessage ValidateErrorMessage(IEntityMessages message)
        {
            return (InformationMessage)message;
        }
    }
}
