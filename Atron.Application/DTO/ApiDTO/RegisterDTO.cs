using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Atron.Application.DTO.ApiDTO
{
    public class RegisterDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Código de acesso é obrigatório")]
        public string Codigo { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Sobrenome é obrigatório")]
        public string Sobrenome { get; set; }
        public DateTime? DataNascimento { get; set; }

        [Required(ErrorMessage ="E-mail é obrigatório")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Senha é obrigatória")]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        [DataType(DataType.Password)]
        [DisplayName("Confirme a senha")]
        [Compare(nameof(Senha), ErrorMessage = "As senhas informadas não são iguais")]
        public string ConfirmaSenha { get; set; }        
    }
}