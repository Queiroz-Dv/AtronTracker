using System.ComponentModel.DataAnnotations;

namespace Application.DTO.Request
{
    public class AlterarEmailRequest
    {
        [Required]
        public string EmailNovo { get; set; }
    }
}
