namespace Atron.Tracker.Domain.ApiEntities
{
    public class ApiRegister
    {
        public ApiRegister() { }

        public ApiRegister(string userName, string email, string password, string confirmPassword)
        {
            UserName = userName;
            Email = email;
            Password = password;
            ConfirmPassword = confirmPassword;
        }

        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
