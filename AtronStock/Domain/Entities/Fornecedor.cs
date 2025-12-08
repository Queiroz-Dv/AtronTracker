using AtronStock.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace AtronStock.Domain.Entities
{
    public class Fornecedor
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(25)]
        public string Codigo { get; set; } = string.Empty; // Ex: GGL, MCR

        [Required, MaxLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required, MaxLength(14)]
        public string CNPJ { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string Telefone { get; set; } = string.Empty;

        public EnderecoVO Endereco { get; set; } = new();
    }
}
