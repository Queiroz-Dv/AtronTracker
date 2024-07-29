namespace Shared.DTO
{
    public class PageInfoDTO<T>
    {
        public PageInfoDTO()
        {
            Entities = new List<T>();
        }

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

        public List<T> Entities { get; set; }
    }
}
