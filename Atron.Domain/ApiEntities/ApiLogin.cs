namespace Atron.Domain.ApiEntities
{
    public class ApiLogin
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool Authenticated { get; set; }
    }
}