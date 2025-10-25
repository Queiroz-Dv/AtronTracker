using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public sealed class Venda
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime DataVenda { get; set; }

        [Required]
        public int QuantidadeDeProdutoVendido { get; set; }

        [Required]
        public decimal PrecoDoProduto { get; set; }

        public bool Removido { get; set; }

        public DateTime? RemovidoEm { get; set; }

        [Required]
        public int ProdutoId { get; set; }

        [Required]
        public string ProdutoCodigo { get; set; }

        public List<Produto> Produtos { get; set; }

        [ForeignKey(nameof(ProdutoCodigo))]
        public Produto Produto { get; set; }

        [Required]
        public int CategoriaId { get; set; }

        [Required]
        public string CategoriaCodigo { get; set; }

        [ForeignKey(nameof(CategoriaCodigo))]
        public Categoria Categoria { get; set; }

        [Required]
        public int ClienteId { get; set; }

        [Required]
        public string ClienteCodigo { get; set; }

        [ForeignKey(nameof(ClienteCodigo))]
        public Cliente Cliente { get; set; }

    }
}