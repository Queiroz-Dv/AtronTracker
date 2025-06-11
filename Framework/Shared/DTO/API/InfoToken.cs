namespace Shared.DTO.API
{
    public class InfoToken
    {
        public string Token { get; set; }

        public DateTime Expires { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime RefreshTokenExpireTime { get; set; }
    }
}