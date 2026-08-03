namespace Domain.ApiEntities
{
    public class ApiLogin
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool Remember { get; set; }
    }
}