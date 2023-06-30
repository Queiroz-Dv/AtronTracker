namespace HLP.Interfaces
{
    /// <summary>
    /// Interface para notificação de campos vazios.
    /// </summary>
    public interface IFieldValidate
    {
        /// <summary>
        /// Adiciona uma messagem se o campo especificado e o segundo campo estão vazios.
        /// </summary>
        /// <param name="fieldName">O nome do campo a ser notificado.</param>
        /// <param name="secondField">O nome do segundo campo a ser notificado.</param>
        object FieldIsEmptyMessage(object fieldName, object secondField);

        /// <summary>
        /// Adiciona uma messagem se o campo especificado, segundo campo e terceiro campo estão vazios.
        /// </summary>
        /// <param name="fieldName">O nome do campo a ser notificado.</param>
        /// <param name="secondField">O nome do segundo campo a ser notificado.</param>
        /// <param name="thirdField">O nome do terceiro campo a ser notificado.</param>
        object FieldIsEmptyMessage(object fieldName, object secondField, object thirdField);

        /// <summary>
        /// Adiciona uma messagem se o campo especificado, segundo campo, terceiro campo e quarto campo estão vazios.
        /// </summary>
        /// <param name="fieldName">O nome do campo a ser notificado.</param>
        /// <param name="secondField">O nome do segundo campo a ser notificado.</param>
        /// <param name="thirdField">O nome do terceiro campo a ser notificado.</param>
        /// <param name="fourthField">O nome do quarto campo a ser notificado.</param>
        object FieldIsEmptyMessage(object fieldName, object secondField, object thirdField, object fourthField);

        /// <summary>
        /// Adiciona uma messagem se o campo especificado, segundo campo, terceiro campo, quarto campo e quinto campo estão vazios.
        /// </summary>
        /// <param name="fieldName">O nome do campo a ser notificado.</param>
        /// <param name="secondField">O nome do segundo campo a ser notificado.</param>
        /// <param name="thirdField">O nome do terceiro campo a ser notificado.</param>
        /// <param name="fourthField">O nome do quarto campo a ser notificado.</param>
        /// <param name="fifthField">O nome do quinto campo a ser notificado.</param>
        object FieldIsEmptyMessage(object fieldName, object secondField, object thirdField, object fourthField, object fifthField);
    }

}