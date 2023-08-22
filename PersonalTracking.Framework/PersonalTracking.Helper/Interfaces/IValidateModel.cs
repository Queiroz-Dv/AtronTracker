namespace PersonalTracking.Helper.Interfaces
{
    public interface IValidateModel
    {
        bool IsValidModel { get; }

        void Validate();
    }
}
