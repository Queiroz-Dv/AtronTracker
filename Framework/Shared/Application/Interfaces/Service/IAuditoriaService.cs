using Shared.Domain.Entities;
using Shared.Domain.ValueObjects;

namespace Shared.Application.Interfaces.Service
{
    public interface IAuditoriaService
    {
        /// <summary>
        /// Registra o início do ciclo de vida de um registro (Criação).
        /// </summary>
        Task<Resultado> RegistrarAuditoriaAsync(string codigoRegistro, string usuarioLogado = null);

        Task<Resultado> RegistrarAuditoriaAsync(string codigoRegistro, string usuarioLogado = null, string historicoDescricao =  null);

        /// <summary>
        /// Atualiza o cabeçalho de auditoria informando a última alteração.
        /// </summary>
        Task<Resultado> RegistrarAlteracaoAuditoriaAsync(string codigoRegistro, string usuarioLogado = null);

        Task<Resultado> RegistrarAlteracaoAuditoriaAsync(string codigoRegistro, string? usuario = null, string historicoDescricao = null);

        /// <summary>
        /// Marca o registro como removido (Soft Delete na auditoria).
        /// </summary>
        Task<Resultado> RegistrarRemocaoAsync(string codigoRegistro, string usuarioLogado = null);

        Task<Resultado> RegistrarRemocaoAsync(string codigoRegistro, string usuarioLogado = null, string historicoDescricao = null);

        Task<Resultado<Auditoria>> ObterPorCodigoRegistro(string codigoRegistro);
    }
}