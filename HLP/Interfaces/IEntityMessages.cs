using System.Windows.Forms;

namespace HLP.Interfaces
{
    public interface IEntityMessages
    {
        /// <summary>
        /// Pergunta ao usuário se deseja atualizar a entidade, com base em uma determinada condição e nome do campo.
        /// </summary>
        /// <param name="condition">Condição que determina se a pergunta será exibida.</param>
        /// <param name="fieldName">Nome do campo relacionado à entidade.</param>
        /// <returns>O resultado da resposta do usuário (DialogResult).</returns>
        DialogResult UpdatedEntityQuestion(bool condition, string fieldName);

        /// <summary>
        /// Pergunta ao usuário se deseja excluir a entidade, com base em uma determinada condição e nome do campo.
        /// </summary>
        /// <param name="condition">Condição que determina se a pergunta será exibida.</param>
        /// <param name="fieldName">Nome do campo relacionado à entidade.</param>
        /// <returns>O resultado da resposta do usuário (DialogResult).</returns>
        DialogResult DeleteEntityQuestion(bool condition, string fieldName);

        /// <summary>
        /// Notifica o usuário que a entidade foi excluída com sucesso, com base em uma determinada condição e nome do campo.
        /// </summary>
        /// <param name="condition">Condição que determina se a notificação será exibida.</param>
        /// <param name="fieldName">Nome do campo relacionado à entidade.</param>
        void EntityDeletedWithSuccess(bool condition, string fieldName);

        /// <summary>
        /// Notifica o usuário que o campo está vazio, com base em uma determinada condição e nome do campo.
        /// </summary>
        /// <param name="condition">Condição que determina se a notificação será exibida.</param>
        /// <param name="fieldName">Nome do campo relacionado à entidade.</param>
        void FieldIsEmpty(bool condition, string fieldName);

        /// <summary>
        /// Notifica o usuário que o campo possui um número mínimo inválido de caracteres, com base em uma determinada condição e nome do campo.
        /// </summary>
        /// <param name="condition">Condição que determina se a notificação será exibida.</param>
        /// <param name="fieldName">Nome do campo relacionado à entidade.</param>
        void InvalidMinimumAmountCharacters(bool condition, string fieldName);

        /// <summary>
        /// Notifica o usuário que a entidade foi salva com sucesso, com base em uma determinada condição e nome do campo.
        /// </summary>
        /// <param name="condition">Condição que determina se a notificação será exibida.</param>
        /// <param name="fieldName">Nome do campo relacionado à entidade.</param>
        void EntitySavedWithSuccess(bool condition, string fieldName);

        /// <summary>
        /// Notifica o usuário que a entidade foi atualizada, com base em uma determinada condição e nome do campo.
        /// </summary>
        /// <param name="condition">Condição que determina se a notificação será exibida.</param>
        /// <param name="fieldName">Nome do campo relacionado à entidade.</param>
        void EntityUpdated(bool condition, string fieldName);

        /// <summary>
        /// Notifica o usuário que a seleção do item é inválida, com base em uma determinada condição.
        /// </summary>
        /// <param name="condition">Condição que determina se a notificação será exibida.</param>
        void InvalidItemSelected(bool condition);
    }
}
