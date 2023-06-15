using System.Windows.Forms;

namespace HLP.Interfaces
{
    public interface IEntityMessages
    {
        /// <summary>
        /// Pergunta ao usuário se deseja atualizar a entidade, com base em uma determinada condição e nome do campo.
        /// </summary>
        /// <param name="fieldName">Nome do campo relacionado à entidade.</param>
        /// <returns>O resultado da resposta do usuário (DialogResult).</returns>
        DialogResult UpdatedEntityQuestion(string fieldName);

        /// <summary>
        /// Pergunta ao usuário se deseja excluir a entidade, com base em uma determinada condição e nome do campo.
        /// </summary>
        /// <param name="fieldName">Nome do campo relacionado à entidade.</param>
        /// <returns>O resultado da resposta do usuário (DialogResult).</returns>
        DialogResult DeleteEntityQuestion(string fieldName);

        /// <summary>
        /// Notifica o usuário que a entidade foi excluída com sucesso, com base em uma determinada condição e nome do campo.
        /// </summary>
        /// <param name="fieldName">Nome do campo relacionado à entidade.</param>
        void EntityDeletedWithSuccess(string fieldName);

        /// <summary>
        /// Notifica o usuário que o campo está vazio, com base em uma determinada condição e nome do campo.
        /// </summary>
        /// <param name="fieldName">Nome do campo relacionado à entidade.</param>
        void FieldIsEmpty(string fieldName);

        /// <summary>
        /// Notifica o usuário que o campo possui um número mínimo inválido de caracteres, com base em uma determinada condição e nome do campo.
        /// </summary>
        /// <param name="fieldName">Nome do campo relacionado à entidade.</param>
        void InvalidMinimumAmountCharacters(string fieldName);

        /// <summary>
        /// Notifica o usuário que a entidade foi salva com sucesso, com base em uma determinada condição e nome do campo.
        /// </summary>
        /// <param name="fieldName">Nome do campo relacionado à entidade.</param>
       object EntitySavedWithSuccess(string fieldName);

        /// <summary>
        /// Notifica o usuário que a entidade foi atualizada, com base em uma determinada condição e nome do campo.
        /// </summary>
        /// <param name="fieldName">Nome do campo relacionado à entidade.</param>
        void EntityUpdated(string fieldName);

        /// <summary>
        /// Notifica o usuário que a seleção do item é inválida, com base em uma determinada condição.
        /// </summary>
        void InvalidItemSelected();

        void EntityInUse(string fieldName);

        void EntityCanBeUse(string fieldName);
    }
}
