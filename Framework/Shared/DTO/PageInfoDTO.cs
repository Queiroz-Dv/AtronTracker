namespace Shared.DTO
{
    public class PageInfoDTO
    {        
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; } = 5;
        public int CurrentPage { get; set; }
        public int TotalPages => (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage);
        public string? Filter { get; set; }
        public string? CurrentController { get; set; }
        public string? Action { get; set; }
        public int StartPage { get; set; }
        public int EndPage { get; set; }    
    }
}