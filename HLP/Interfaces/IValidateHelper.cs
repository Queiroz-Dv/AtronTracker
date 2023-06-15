using HLP.Entity;

namespace HLP.Interfaces
{
    public interface IValidateHelper
    {
        bool Validate(bool condition);

        InformationMessage ValidateErrorMessage(InformationMessage message);
    }
}