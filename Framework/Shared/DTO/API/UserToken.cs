namespace Shared.DTO.API
{
    public class UserToken
    {
        public string Token { get; set; }
        public DateTime Expires { get; set; }
    }
}