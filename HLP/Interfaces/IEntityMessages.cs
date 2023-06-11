using System.Windows.Forms;

namespace HLP.Interfaces
{
    public interface IEntityMessages
    {
        DialogResult UpdatedEntityQuestion(bool condition, string fieldName);

        DialogResult DeleteEntityQuestion(bool condition, string fieldName);

        void EntityDeletedWithSuccess(bool condition, string fieldName);

        void FieldIsEmpty(bool condition, string fieldName);

        void InvalidMinimumAmountCharacters(bool condition, string fieldName);
    }
}
