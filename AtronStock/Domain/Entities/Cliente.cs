using AtronStock.Domain.Enums;
using AtronStock.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace AtronStock.Domain.Entities
{
    public sealed class Cliente
    {
        [Key] public int Id { get; set; }

        [MaxLength(25), Required]
        public string Codigo { get; set; } = string.Empty;

        [MaxLength(50), Required]
        public string Nome { get; set; } = string.Empty;

        [MaxLength(11)]
        public string CPF { get; set; } = string.Empty;

        [MaxLength(14)]
        public string CNPJ { get; set; } = string.Empty;

        public EStatus Status { get; set; }

        public EnderecoVO Endereco { get; set; } = new();

        [MaxLength(50), Required]
        public string Email { get; set; } = string.Empty;

        [MaxLength(15), Required]
        public string Telefone { get; set; } = string.Empty;

        public Cliente()
        {
            Status = EStatus.Ativo;
            Endereco = new EnderecoVO();
        }
    }
}