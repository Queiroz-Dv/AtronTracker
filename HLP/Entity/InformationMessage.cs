using HLP.Interfaces;
using System.Windows.Forms;
using static HLP.Helpers.MessageHelper;

namespace HLP.Entity
{
    public class InformationMessage : IEntityMessages
    {
        public DialogResult DeleteEntityQuestion(bool condition, string fieldName)
        {
            return ShowMessage(condition,
                        $"Are you sure to delete this {fieldName}?",
                        MessageBoxButtons.YesNo,
                        EnumLevelMessage.Question);
        }

        public void EntityDeletedWithSuccess(bool condition, string fieldName)
        {
            ShowMessage(condition,
                        $"{fieldName} was deleted successfully!",
                        MessageBoxButtons.OK,
                        EnumLevelMessage.Information);
        }

        public void EntitySavedWithSuccess(bool condition, string fieldName)
        {
            ShowMessage(condition,
                        $"The {fieldName} was saved successfully!",
                        MessageBoxButtons.OK,
                        EnumLevelMessage.Information);
        }

        public void EntityUpdated(bool condition, string fieldName)
        {
            ShowMessage(condition,
                    $"{fieldName} was deleted successfully!",
                    MessageBoxButtons.OK,
                    EnumLevelMessage.Information);
        }

        public void FieldIsEmpty(bool condition, string fieldName)
        {
            ShowMessage(condition,
                        $"Please fill the {fieldName}",
                        MessageBoxButtons.OK,
                        EnumLevelMessage.Warning);
        }

        public void InvalidItemSelected(bool condition)
        {
            ShowMessage(condition,
                    $"Selected item invalid, try again.",
                    MessageBoxButtons.OK,
                    EnumLevelMessage.Warning);
        }

        public void InvalidMinimumAmountCharacters(bool condition, string fieldName)
        {
            ShowMessage(condition,
                        $"The name of {fieldName} is incorrect because the minimum amount of characters is 3.",
                        MessageBoxButtons.OK,
                        EnumLevelMessage.Warning);
        }

        public DialogResult UpdatedEntityQuestion(bool condition, string fieldName)
        {
            return ShowMessage(condition,
                        $"Are you sure to change the {fieldName}?",
                        MessageBoxButtons.YesNo,
                        EnumLevelMessage.Question);
        }
    }
}
