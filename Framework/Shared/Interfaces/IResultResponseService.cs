using Shared.DTO;

namespace Shared.Interfaces
{
    /// <summary>
    /// Interface para o servi�o de resposta
    /// </summary>
    public interface IResultResponseService
    {
         List<ResultResponseDTO> ResultMessages { get; set; }

        void AddMessage(string message);

        void AddError(string message);

        void AddWarning(string message);
    }
}