using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Atron.Domain.Entities
{
    public sealed class Produto
    {
        [Key] public int Id { get; set; }

        [MaxLength(25), Required]
        public string Codigo { get; set; }

        [MaxLength(50), Required]
        public string Descricao { get; set; }

        public int CategoriaId { get; set; }

        [MaxLength(25), Required] public string CategoriaCodigo { get; set; }

        [ForeignKey(nameof(CategoriaCodigo))]
        public Categoria Categoria { get; set; }

        [Required]
        public int QuantidadeEmEstoque { get; set; }

        public bool Removido { get; set; }

        public DateTime? RemovidoEm { get; set; }

        public Venda Venda { get; set; }

        public List<Venda> Vendas { get; set; }
    }
}