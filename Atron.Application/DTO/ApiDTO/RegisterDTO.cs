using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Atron.Application.DTO.ApiDTO
{
    public class RegisterDTO : FactoryDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; }

        public string Sobrenome { get; set; }
        public DateTime? DataNascimento { get; set; }

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
