using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Atron.Application.DTO.Account
{
    public class RegisterDTO : FactoryDTO
    {
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }


        [Required]
        [DataType(DataType.Password)]
        public string Passsword { get; set; }


        [DataType(DataType.Password)]
        [DisplayName("Confirme a senha")]
        [Compare(nameof(Passsword), ErrorMessage = "As senhas informadas não são iguais")]
        public string ConfirmPasssword { get; set; }
        public bool RegisterConfirmed { get; set; }
    }
}
