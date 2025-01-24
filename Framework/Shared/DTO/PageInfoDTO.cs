namespace Shared.DTO
{
    /// <summary>
    /// Classe que define a estrutura de informações para paginação das entidades
    /// </summary>
    public class PageInfoDTO
    {        
        public int StartPage { get; set; }
        public int EndPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; } = 5;
        public int TotalPages => (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage);        


        public PageRequestInfoDTO? PageRequestInfo { get; set; }
    }

    public class PageRequestInfoDTO
    {
        public string CurrentViewController { get; set; }
        public string Action { get; set; } = nameof(Index);
        public string Parameter { get; set; }
        public string Filter { get; set; }
        public string KeyToSearch { get; set; }


        public string ApiControllerName { get; set; }
        public string ApiControllerAction { get; set; }
    }
}