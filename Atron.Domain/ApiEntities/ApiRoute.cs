namespace Atron.Domain.ApiEntities
{
    public class ApiRoute
    {
        public int Id { get; set; }
        public string ModuleName { get; set; }
        public string RouteUrl { get; set; }
        public string HttpMethod { get; set; }
        public bool IsActive { get; set; }
    }
}