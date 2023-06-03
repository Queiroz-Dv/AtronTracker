using NTF.ErrorsType;
using System;

namespace NTF.Entities
{
    /// <summary>
    /// Classe que representa um modelo de entidade genérico.
    /// <br></br>
    /// Contém propriedades e métodos para validação e gerenciamento de erros.
    /// </summary>
    public class EntityModel
    {
        public Error Errors { get; } = new Error();

        /// <summary>
        /// Realiza a validação do modelo de entidade.
        /// <br></br>
        /// Pode ser sobrescrito nas classes derivadas para adicionar lógica de validação personalizada.
        /// </summary>
        public virtual void Validate() { }

        /// <summary>
        /// Verifica uma condição e adiciona um erro à lista de erros se a condição for verdadeira.
        /// </summary>
        /// <param name="condition">Condição a ser verificada.</param>
        /// <param name="description">Descrição do erro a ser adicionado.</param>
        protected void Fail(bool condition, ErrorDescription description)
        {
            if (condition)
            {
                Errors.Add(description);
            }
        }

        /// <summary>
        /// Verifica se o modelo de entidade é válido, ou seja, se não possui erros.
        /// </summary>
        /// <returns>True se não houver erros, False caso contrário.</returns>
        public bool IsValid()
        {
            return !Errors.HasErrors;
        }

        #region Validations

        /// <summary>
        /// Verifica se o Id é inválido e adiciona um erro à lista de erros, se for o caso.
        /// </summary>
        /// <param name="Id">Guid de identificação.</param>
        /// <param name="error">Descrição do erro a ser adicionado.</param>
        protected void IsInvalidId(int Id, ErrorDescription error)
        {
            Fail(Id == 0, error);
        }

        /// <summary>
        /// Verifica se o nome é inválido e adiciona um erro à lista de erros, se for o caso.
        /// </summary>
        /// <param name="entityName">Nome a ser verificado.</param>
        /// <param name="error">Descrição do erro a ser adicionado.</param>
        protected void IsInvalidName(string entityName, ErrorDescription error)
        {
            Fail(string.IsNullOrWhiteSpace(entityName), error);
        }

        #endregion

        #region Errors

        public static ErrorDescription InvalidId = new ErrorDescription("Invalid Id", new Critical());
        public static ErrorDescription InvalidName = new ErrorDescription("Invalid Name", new Critical());

        #endregion
    }
}
