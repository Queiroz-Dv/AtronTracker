using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Atron.Application.DTO.ApiDTO
{
    public class RegisterDTO
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Nome { get; set; }

        public string Sobrenome { get; set; }
        public DateTime? DataNascimento { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        [DataType(DataType.Password)]
        [DisplayName("Confirme a senha")]
        [Compare(nameof(Senha), ErrorMessage = "As senhas informadas não são iguais")]
        public string ConfirmaSenha { get; set; }

        public string GerarUserName()
        {
            return $"{Nome}@{Sobrenome}";
        }
    }
}