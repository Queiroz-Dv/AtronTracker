namespace HLP.Interfaces
{
    public interface IValidateModel
    {
        bool IsValidModel { get; }

        void Validate();
    }
}
