using Shared.Domain.Entities;
using Shared.Domain.ValueObjects;

namespace Shared.Application.Interfaces.Service
{
    public interface IAuditoriaService
    {
        /// <summary>
        /// Registra o início do ciclo de vida de um registro (Criação).
        /// </summary>
        Task<Resultado> RegistrarCriacaoAsync(string codigoRegistro, string usuario = null);

        /// <summary>
        /// Atualiza o cabeçalho de auditoria informando a última alteração.
        /// </summary>
        Task<Resultado> RegistrarAlteracaoAsync(string codigoRegistro, string usuario = null);

        /// <summary>
        /// Marca o registro como removido (Soft Delete na auditoria).
        /// </summary>
        Task<Resultado> RegistrarRemocaoAsync(string codigoRegistro, string usuario = null);

        Task<Resultado<Auditoria>> ObterPorCodigoRegistro(string codigoRegistro);
    }
}