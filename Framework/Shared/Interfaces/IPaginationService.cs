using Shared.DTO;

namespace Shared.Interfaces
{
    /// <summary>
    /// Interface para o serviço de paginação
    /// </summary>
    /// <typeparam name="T">Entidade que é utilizada no processo.</typeparam>
    public interface IPaginationService<T>
    {
        void Paginate(List<T> items, int itemPage, string controllerName, string filter, string action = nameof(Index));
        void ConfigureEntityPaginated(List<T> items, string filter);
        List<T> GetEntitiesFilled();
        PageInfoDTO PageInfo { get; }
        string FilterBy { get; set; }
        bool ForceFilter { get; set; }
    }
}