using HLP.Interfaces;
using System.Windows.Forms;
using static HLP.Helpers.MessageHelper;

namespace HLP.Entity
{
    /// <summary>
    /// Classe que implementa as notificações de mensagens para os formulários
    /// </summary>
    public class InformationMessage : IEntityMessages
    {

        public DialogResult DeleteEntityQuestionMessage(string fieldName)
        {
            return ShowMessage($"Are you sure to delete this {fieldName}?",
                               MessageBoxButtons.YesNo,
                               EnumLevelMessage.Question);
        }

        public void EntityCanBeUseMessage(string fieldName)
        {
            ShowMessage($"This {fieldName} is usable",
                        MessageBoxButtons.OK,
                        EnumLevelMessage.Information);
        }

        public void EntityDeletedWithSuccessMessage(string fieldName)
        {
            ShowMessage($"{fieldName} was deleted successfully!",
                        MessageBoxButtons.OK,
                        EnumLevelMessage.Information);
        }

        public void EntityInUseMessage(string fieldName)
        {
            ShowMessage($"This {fieldName} is used by another employee please change it",
                        MessageBoxButtons.OK,
                        EnumLevelMessage.Information);
        }

        public object EntitySavedWithSuccessMessage(string fieldName)
        {
            return ShowMessage($"The {fieldName} was saved successfully!",
                        MessageBoxButtons.OK,
                        EnumLevelMessage.Information);
        }

        public void EntityUpdatedMessage(string fieldName)
        {
            ShowMessage($"{fieldName} was deleted successfully!",
                        MessageBoxButtons.OK,
                        EnumLevelMessage.Information);
        }

        public void FieldIsEmptyMessage(string fieldName)
        {
            ShowMessage($"Please fill the {fieldName}",
                        MessageBoxButtons.OK,
                        EnumLevelMessage.Warning);
        }

        public void FieldIsEmptyMessage(string fieldName, string secondField)
        {
            ShowMessage($"Please fill the {fieldName} and {secondField}",
                        MessageBoxButtons.OK,
                        EnumLevelMessage.Warning);
        }

        public void FieldIsEmptyMessage(string fieldName, string secondField, string thirdField)
        {
            ShowMessage($"Please fill the {fieldName}, {secondField} and {thirdField}",
                        MessageBoxButtons.OK,
                        EnumLevelMessage.Warning);
        }

        public void FieldIsEmptyMessage(string fieldName, string secondField,
                                        string thirdField, string fourthField)
        {
            ShowMessage($"Please fill the {fieldName}, {secondField}, {thirdField} and {fourthField}",
                        MessageBoxButtons.OK,
                        EnumLevelMessage.Warning);
        }

        public void FieldIsEmptyMessage(string fieldName, string secondField,
                                        string thirdField, string fourthField,
                                        string fifthField)
        {
            ShowMessage($"Please fill the {fieldName}, {secondField}, {thirdField}, {fourthField}, and {fifthField}",
                        MessageBoxButtons.OK,
                        EnumLevelMessage.Warning);
        }

        public void InvalidItemSelectedMessage()
        {
            ShowMessage($"Selected item invalid, try again.",
                        MessageBoxButtons.OK,
                        EnumLevelMessage.Warning);
        }

        public void InvalidMinimumAmountCharactersMessage(string fieldName)
        {
            ShowMessage($"The name of {fieldName} is incorrect because the minimum amount of characters is 3.",
                        MessageBoxButtons.OK,
                        EnumLevelMessage.Warning);
        }

        public DialogResult UpdatedEntityQuestionMessage(string fieldName)
        {
            return ShowMessage($"Are you sure to change the {fieldName}?",
                               MessageBoxButtons.YesNo,
                               EnumLevelMessage.Question);
        }
    }
}