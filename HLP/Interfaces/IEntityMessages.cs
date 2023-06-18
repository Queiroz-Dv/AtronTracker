using System.Windows.Forms;

namespace HLP.Interfaces
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
        /// <returns>O resultado da resposta do usuário (DialogResult).</returns>
        DialogResult UpdatedEntityQuestionMessage(string fieldName);

        /// <summary>
        /// Pergunta ao usuário se deseja excluir a entidade, com base em uma determinada condição e nome do campo.
        /// </summary>
        /// <param name="fieldName">Nome do campo relacionado à entidade.</param>
        /// <returns>O resultado da resposta do usuário (DialogResult).</returns>
        DialogResult DeleteEntityQuestionMessage(string fieldName);

        /// <summary>
        /// Notifica o usuário que a entidade foi excluída com sucesso, com base em uma determinada condição e nome do campo.
        /// </summary>
        /// <param name="fieldName">Nome do campo relacionado à entidade.</param>
        void EntityDeletedWithSuccessMessage(string fieldName);

        /// <summary>
        /// Notifica o usuário que o campo está vazio, com base em uma determinada condição e nome do campo.
        /// </summary>
        /// <param name="fieldName">Nome do campo relacionado à entidade.</param>
        void FieldIsEmptyMessage(string fieldName);

        /// <summary>
        /// Notifica o usuário que o campo possui um número mínimo inválido de caracteres, com base em uma determinada condição e nome do campo.
        /// </summary>
        /// <param name="fieldName">Nome do campo relacionado à entidade.</param>
        void InvalidMinimumAmountCharactersMessage(string fieldName);

        /// <summary>
        /// Notifica o usuário que a entidade foi salva com sucesso, com base em uma determinada condição e nome do campo.
        /// </summary>
        /// <param name="fieldName">Nome do campo relacionado à entidade.</param>
       object EntitySavedWithSuccessMessage(string fieldName);

        /// <summary>
        /// Notifica o usuário que a entidade foi atualizada, com base em uma determinada condição e nome do campo.
        /// </summary>
        /// <param name="fieldName">Nome do campo relacionado à entidade.</param>
        void EntityUpdatedMessage(string fieldName);

        /// <summary>
        /// Exibe uma mensagem indicando que a seleção do item é inválida.
        /// </summary>
        void InvalidItemSelectedMessage();

        /// <summary>
        /// Exibe uma mensagem indicando que a entidade está em uso.
        /// </summary>
        /// <param name="fieldName">O nome do campo ou entidade relacionada que está em uso.</param>
        void EntityInUseMessage(string fieldName);

        /// <summary>
        /// Exibe uma mensagem indicando que a entidade pode ser usada.
        /// </summary>
        /// <param name="fieldName">O nome do campo ou entidade relacionada que pode ser usada.</param>
        void EntityCanBeUseMessage(string fieldName);
    }
}
