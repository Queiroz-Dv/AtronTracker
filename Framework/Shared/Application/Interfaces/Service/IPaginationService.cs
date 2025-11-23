using Shared.Application.DTOS.Common;

namespace Shared.Application.Interfaces.Service
{
    /// <summary>
    /// Interface para os serviços e processos de paginação
    /// </summary>
    /// <typeparam name="T">Entidade que é utilizada no processo.</typeparam>
    public interface IPaginationService<T>
    {
        void ConfigurePagination(List<T> itens, PageInfoDTO pageInfo);
        PageInfoDTO GetPageInfo();
        List<T> GetEntitiesFilled();
    }
}