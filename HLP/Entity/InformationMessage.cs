using HLP.Enums;
using HLP.Helpers;
using HLP.Interfaces;
using System.Windows.Forms;

namespace HLP.Entity
{
    /// <summary>
    /// Classe que implementa as notificações de mensagens para os formulários
    /// </summary>
    public class InformationMessage : IEntityMessages
    {
        IMessageHelper messageHelper;

        public InformationMessage()
        {
            messageHelper = new MessageHelper();
        }

        public object DeleteEntityQuestionMessage(string fieldName)
        {
            var message = messageHelper.ShowMessage($"Are you sure to delete this {fieldName}?",
                               MessageBoxButtons.YesNo,
                               EnumLevelMessage.Question);
            return message;
        }

        public object EntityCanBeUseMessage(string fieldName)
        {
            var message = messageHelper.ShowMessage($"This {fieldName} is usable",
                        MessageBoxButtons.OK,
                        EnumLevelMessage.Information);

            return message;
        }

        public object EntityDeletedWithSuccessMessage(string fieldName)
        {
            var message = messageHelper.ShowMessage($"{fieldName} was deleted successfully!",
                        MessageBoxButtons.OK,
                        EnumLevelMessage.Information);

            return message;
        }

        public object EntityInUseMessage(string fieldName)
        {
            var message = messageHelper.ShowMessage($"This {fieldName} is used by another employee please change it",
                                                    MessageBoxButtons.OK,
                                                    EnumLevelMessage.Information);
            return message;
        }

        public object EntitySavedWithSuccessMessage(string fieldName)
        {
            var message = messageHelper.ShowMessage($"The {fieldName} was saved successfully!",
                                                    MessageBoxButtons.OK,
                                                    EnumLevelMessage.Information);
            return message;
        }

        public object EntityUpdatedMessage(string fieldName)
        {
            var message = messageHelper.ShowMessage($"{fieldName} was updated successfully!",
                                                    MessageBoxButtons.OK,
                                                    EnumLevelMessage.Information);
            return message;
        }

        public object FieldIsEmptyMessage(object fieldName)
        {
            var fieldConverted = fieldName?.ToString();
            var message = messageHelper.ShowMessage($"Please fill the {fieldConverted}",
                        MessageBoxButtons.OK,
                        EnumLevelMessage.Warning);

            return message;
        }

        public object FieldIsEmptyMessage(object fieldName, object secondField)
        {
            var fieldNameLabel = fieldName as Label;
            var secondFieldLabel = secondField as Label;

            var fieldNameConverted = fieldNameLabel.Text;
            var secondFieldConverted = secondFieldLabel.Text;

            var message = messageHelper.ShowMessage($"Please fill the {fieldNameConverted} and {secondFieldConverted}",
                        MessageBoxButtons.OK,
                        EnumLevelMessage.Warning);

            return message;
        }

        public object FieldIsEmptyMessage(object fieldName, object secondField, object thirdField)
        {
            var fieldNameConverted = fieldName?.ToString();
            var secondConverted = secondField?.ToString();
            var thirdConverted = thirdField?.ToString();

            var message = messageHelper.ShowMessage($"Please fill the {fieldNameConverted}, {secondConverted} and {thirdConverted}",
                        MessageBoxButtons.OK,
                        EnumLevelMessage.Warning);

            return message;
        }

        public object FieldIsEmptyMessage(object fieldName, object secondField, object thirdField, object fourthField)
        {
            var fieldNameConverted = fieldName?.ToString();
            var secondConverted = secondField?.ToString();
            var thirdConverted = thirdField?.ToString();
            var fourthFieldConverted = fourthField?.ToString();

            var message = messageHelper.ShowMessage($"Please fill the {fieldNameConverted}, {secondConverted}, {thirdConverted} and {fourthFieldConverted}",
                                                    MessageBoxButtons.OK,
                                                    EnumLevelMessage.Warning);
            return message;
        }

        public object FieldIsEmptyMessage(object fieldName, object secondField, object thirdField, object fourthField, object fifthField)
        {
            var fieldNameConverted = fieldName?.ToString();
            var secondConverted = secondField?.ToString();
            var thirdConverted = thirdField?.ToString();
            var fourthFieldConverted = fourthField?.ToString();
            var fifthFieldConverted = fifthField?.ToString();

            var message = messageHelper.ShowMessage($"Please fill the {fieldNameConverted}, {secondConverted}, {thirdConverted}, {fourthFieldConverted}, and {fifthFieldConverted}",
                                                    MessageBoxButtons.OK,
                                                    EnumLevelMessage.Warning);
            return message;
        }

        public object InvalidItemSelectedMessage()
        {
            var message = messageHelper.ShowMessage($"Selected item invalid, try again.",
                        MessageBoxButtons.OK,
                        EnumLevelMessage.Warning);

            return message;
        }

        public object InvalidMinimumAmountCharactersMessage(string fieldName)
        {
            var message = messageHelper.ShowMessage($"The name of {fieldName} is incorrect because the minimum amount of characters is 3.",
                                                    MessageBoxButtons.OK,
                                                    EnumLevelMessage.Warning);
            return message;
        }

        public object UpdatedEntityQuestionMessage(string fieldName)
        {
            return messageHelper.ShowMessage($"Are you sure to change the {fieldName}?",
                               MessageBoxButtons.YesNo,
                               EnumLevelMessage.Question);
        }
    }
}