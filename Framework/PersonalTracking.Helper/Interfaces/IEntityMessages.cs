namespace PersonalTracking.Helper.Interfaces
{
    /// <summary>
    /// Interface com a lista de notificações comuns para os formulários
    /// </summary>
    public interface IEntityMessages : IFieldValidate
    {
        /// <summary>
        /// Pergunta ao usuário se deseja atualizar a entidade, com base em uma determinada condição e nome do campo.
        /// </summary>
        /// <param name="fieldName">Nome do campo relacionado à entidade.</param>
        /// <returns>O resultado da resposta do usuário (object).</returns>
        object UpdatedEntityQuestionMessage(string fieldName);

        /// <summary>
        /// Pergunta ao usuário se deseja excluir a entidade, com base em uma determinada condição e nome do campo.
        /// </summary>
        /// <param name="fieldName">Nome do campo relacionado à entidade.</param>
        /// <returns>O resultado da resposta do usuário (object).</returns>
        object DeleteEntityQuestionMessage(string fieldName);

        /// <summary>
        /// Notifica o usuário que a entidade foi excluída com sucesso, com base em uma determinada condição e nome do campo.
        /// </summary>
        /// <param name="fieldName">Nome do campo relacionado à entidade.</param>
        object EntityDeletedWithSuccessMessage(string fieldName);

        /// <summary>
        /// Notifica o usuário que o campo está vazio, com base em uma determinada condição e nome do campo.
        /// </summary>
        /// <param name="fieldName">Nome do campo relacionado à entidade.</param>
        object FieldIsEmptyMessage(object fieldName);

        /// <summary>
        /// Notifica o usuário que o campo possui um número mínimo inválido de caracteres, com base em uma determinada condição e nome do campo.
        /// </summary>
        /// <param name="fieldName">Nome do campo relacionado à entidade.</param>
        object InvalidMinimumAmountCharactersMessage(string fieldName);

        /// <summary>
        /// Notifica o usuário que a entidade foi salva com sucesso, com base em uma determinada condição e nome do campo.
        /// </summary>
        /// <param name="fieldName">Nome do campo relacionado à entidade.</param>
        object EntitySavedWithSuccessMessage(string fieldName);

        /// <summary>
        /// Notifica o usuário que a entidade foi atualizada, com base em uma determinada condição e nome do campo.
        /// </summary>
        /// <param name="fieldName">Nome do campo relacionado à entidade.</param>
        object EntityUpdatedMessage(string fieldName);

        /// <summary>
        /// Exibe uma mensagem indicando que a seleção do item é inválida.
        /// </summary>
        object InvalidItemSelectedMessage();

        /// <summary>
        /// Exibe uma mensagem indicando que a entidade está em uso.
        /// </summary>
        /// <param name="fieldName">O nome do campo ou entidade relacionada que está em uso.</param>
        object EntityInUseMessage(string fieldName);

        /// <summary>
        /// Exibe uma mensagem indicando que a entidade pode ser usada.
        /// </summary>
        /// <param name="fieldName">O nome do campo ou entidade relacionada que pode ser usada.</param>
        object EntityCanBeUseMessage(string fieldName);
    }
}
