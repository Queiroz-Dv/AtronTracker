using Shared.DTO;

namespace Shared.Interfaces
{
    /// <summary>
    /// Interface para os serviços e processos de paginação
    /// </summary>
    /// <typeparam name="T">Entidade que é utilizada no processo.</typeparam>
    public interface IPaginationService<T>
    {       
        void ConfigurePagination(PageInfoDTO<T> pageInfo);
        void ConfigurePageRequestInfo(string apiControllerName, string apiControllerAction = "", string filter = "");
        PageInfoDTO<T> GetPageInfo();
        PageRequestInfoDTO GetPageRequestInfo();
    }
}