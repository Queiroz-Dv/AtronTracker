using Shared.DTO;
using Shared.Interfaces;

namespace Shared.Services
{
    /// <summary>
    /// Classe de implementação dos serviços de paginação
    /// </summary>
    /// <typeparam name="T">Entidade que será utilizada no processo</typeparam>
    public class PaginationService<T> : IPaginationService<T>
    {
        public PaginationService()
        {
            PageInfo = new PageInfoDTO();
            Entities = new List<T>();
        }

        /// <summary>
        /// Entidades que serão atualizadas e utilizadas ao longo do processo
        /// </summary>
        private List<T> Entities { get; set; }

        /// <summary>
        /// Páginal atual
        /// </summary>
        private int CurrentPage { get; set; }

        /// <summary>
        /// Itens por página
        /// </summary>
        private int ItemsPerPage { get; set; }


        // Propriedades da interface
        public bool ForceFilter { get; set; }
        public string FilterBy { get; set; }
        public PageInfoDTO PageInfo { get; set; }
        
        /// <summary>
        /// Método principal de paginação
        /// </summary>
        /// <typeparam name="T">Tipo da entidade que é utilizada no processo</typeparam>
        /// <param name="allItens">Todos os itens que serão utilizados no processo</param>
        /// <param name="currentPage">Página atual quando o processo inicia</param>
        /// <param name="controllerRoute">Rota do controlador</param>
        /// <param name="filter">Filtro utilizado para buscar os registros de acordo com o valor informado</param>
        /// <param name="action">Nome da action de acordo com o controlador que disparou</param>
        /// <param name="itemsPerPage">Itens por página</param>
        private void Paginate<T>(IEnumerable<T> allItens,
            int currentPage,
            string controllerRoute,
            string filter = "",
            string action = nameof(Index),
            int itemsPerPage = 5)
        {
            var filteredItens = ApplyFilter(allItens, filter);
            var totalItems = filteredItens.Count();
            var totalPages = (int)Math.Ceiling((decimal)totalItems / itemsPerPage);
            var startPage = currentPage - 2;
            var endPage = currentPage + 2;

            if (startPage <= 0)
            {
                endPage = endPage - (startPage - 1);
                startPage = 1;
            }

            if (endPage > totalPages)
            {
                endPage = totalPages;
                if (endPage > 5)
                {
                    startPage = endPage - 4;
                }
            }

            CurrentPage = currentPage;
            ItemsPerPage = itemsPerPage;       

            var pageInfo = new PageInfoDTO
            {
                TotalItems = totalItems,
                ItemsPerPage = itemsPerPage,
                CurrentPage = currentPage,
                CurrentController = controllerRoute,
                Action = action,
                Filter = filter,
                StartPage = startPage,
                EndPage = endPage
            };

            PageInfo = pageInfo;
        }

        public void ConfigureEntityPaginated(List<T> values, string filter = "")
        {
            var filteredItems = ForceFilter ? ApplyFilter(values, FilterBy) : ApplyFilter(values, filter);

            Entities = PaginateEntities(filteredItems);            
        }

        /// <summary>
        /// Método que aplica a filtragem nos itens 
        /// </summary>
        /// <typeparam name="T">Tipo da entidade</typeparam>
        /// <param name="items">Itens que serão processados</param>
        /// <param name="filter">Filtro que será usado para realizar o processo e atualizar a lista</param>
        /// <returns></returns>
        private IEnumerable<T> ApplyFilter<T>(IEnumerable<T> items, string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                return items;
            }

            // Aqui estou obtendo os itens pelo tipo, em seguida obtém a propriedade Codigo e obtenho o valor passando o item
            // Após isso ei verifico se contém alguma entidade de acordo com o filtro informado
            var entities = items.Where(item => item?.GetType().GetProperty("Codigo")?.GetValue(item)?.ToString()?.Contains(filter) ?? false);
            return entities;
        }

        /// <summary>
        /// Método que realiza a operação de paginar as entidades
        /// </summary>
        /// <typeparam name="T">Tipo da entidade processada</typeparam>
        /// <param name="items">Itens que serão processados</param>
        /// <returns>Uma lista de entidades paginadas</returns>
        private List<T> PaginateEntities<T>(IEnumerable<T> items)
        {
            var entities = items.Skip((CurrentPage - 1) * ItemsPerPage)
                                .Take(ItemsPerPage)
                                .ToList();
            return entities;
        }

        public List<T> GetEntitiesFilled()
        {
            return Entities;
        }

        public void Paginate(List<T> items, int itemPage, string controllerName, string filter, string action = nameof(Index))
        {
            Paginate<T>(items, itemPage, controllerName, filter, action);
        }
    }
}