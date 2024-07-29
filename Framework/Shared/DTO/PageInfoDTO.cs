namespace Shared.DTO
{
    public class PageInfoDTO
    {        
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages
        {
            get
            {
                return (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage);
            }
        }

        public string? Filter { get; set; }
        public int StartPage { get; set; }
        public int EndPage { get; set; }      
    }
}
