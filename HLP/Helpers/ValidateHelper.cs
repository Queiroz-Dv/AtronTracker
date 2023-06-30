using HLP.Interfaces;

namespace HLP.Helpers
{
    public abstract class ValidateHelper : IValidateHelper
    {
        public bool FieldValidate(bool condition)
        {
            return condition;
        }
    }
}
