using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Domain.Entities.Identity
{
    public class ApplicationUser : IdentityUser<int>
    {
        [NotMapped]
        public string Token { get; set; }

        public string RefreshToken { get; set; }

        public DateTime RefreshTokenExpireTime { get; set; }
    }
}