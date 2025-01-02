namespace Atron.Domain.ApiEntities
{
    public class Login
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool Authenticated { get; set; }
    }
}