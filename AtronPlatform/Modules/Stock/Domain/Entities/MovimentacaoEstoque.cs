using AtronStock.Domain.Entities;
using AtronStock.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtronStock.Domain.Entities
{
    public class MovimentacaoEstoque
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EstoqueId { get; set; }

        [ForeignKey("EstoqueId")]
        public Estoque Estoque { get; set; } = null!;

        [Required]
        public TipoMovimentacao TipoMovimentacao { get; set; }

        [Required]
        public int Quantidade { get; set; }

        public DateTime DataMovimentacao { get; set; }

        [MaxLength(200)]
        public string? Observacao { get; set; }

        [MaxLength(100)]
        public string? Origem { get; set; } // Ex: "Fornecedor: ABC Ltda" ou "Cliente: João Silva"

        public int? TransacaoId { get; set; } // ID da Entrada ou Venda
    }
}
