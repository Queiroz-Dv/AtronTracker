using System;

namespace Atron.Infrastructure.Interfaces
{
    /// <summary>
    /// Interface para o Unit of Work leve (LiteUnitOfWork).
    /// </summary>
    public interface ILiteUnitOfWork : IDisposable
    {
        /// <summary>
        /// Inicia uma transação.
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// Realiza a confirmação das alterações na transação atual.
        /// </summary>
        void Commit();

        /// <summary>
        /// Revete as alterações na transação atual.
        /// </summary>
        void Rollback();
    }    
}