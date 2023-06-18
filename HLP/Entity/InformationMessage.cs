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

        public void FieldIsEmptyMessage(object fieldName)
        {
            var fieldConverted = fieldName?.ToString();
            ShowMessage($"Please fill the {fieldConverted}",
                        MessageBoxButtons.OK,
                        EnumLevelMessage.Warning);
        }

        public void FieldIsEmptyMessage(object fieldName, object secondField)
        {
            var fieldNameLabel =  fieldName as Label;
            var secondFieldLabel = secondField as Label;

            var fieldNameConverted = fieldNameLabel.Text;
            var secondFieldConverted = secondFieldLabel.Text;

            ShowMessage($"Please fill the {fieldNameConverted} and {secondFieldConverted}",
                        MessageBoxButtons.OK,
                        EnumLevelMessage.Warning);
        }

        public void FieldIsEmptyMessage(object fieldName, object secondField, object thirdField)
        {
            var fieldNameConverted = fieldName?.ToString();
            var secondConverted = secondField?.ToString();
            var thirdConverted = thirdField?.ToString();

            ShowMessage($"Please fill the {fieldNameConverted}, {secondConverted} and {thirdConverted}",
                        MessageBoxButtons.OK,
                        EnumLevelMessage.Warning);
        }

        public void FieldIsEmptyMessage(object fieldName, object secondField, object thirdField, object fourthField)
        {
            var fieldNameConverted = fieldName?.ToString();
            var secondConverted = secondField?.ToString();
            var thirdConverted = thirdField?.ToString();
            var fourthFieldConverted = fourthField?.ToString();

            ShowMessage($"Please fill the {fieldNameConverted}, {secondConverted}, {thirdConverted} and {fourthFieldConverted}",
                        MessageBoxButtons.OK,
                        EnumLevelMessage.Warning);
        }

        public void FieldIsEmptyMessage(object fieldName, object secondField, object thirdField, object fourthField, object fifthField)
        {
            var fieldNameConverted = fieldName?.ToString();
            var secondConverted = secondField?.ToString();
            var thirdConverted = thirdField?.ToString();
            var fourthFieldConverted = fourthField?.ToString();
            var fifthFieldConverted = fifthField?.ToString();

            ShowMessage($"Please fill the {fieldNameConverted}, {secondConverted}, {thirdConverted}, {fourthFieldConverted}, and {fifthFieldConverted}",
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