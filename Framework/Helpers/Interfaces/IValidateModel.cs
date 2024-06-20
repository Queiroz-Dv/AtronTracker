namespace Helpers.Interfaces
{
    public interface IValidateModel
    {
        bool IsValidModel { get; }

        void Validate();
    }
}
