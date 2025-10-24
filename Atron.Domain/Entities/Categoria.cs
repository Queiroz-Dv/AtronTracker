using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Atron.Tracker.Domain.Entities
{
    public sealed class Categoria
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(25), Required]
        public string Codigo { get; set; }

        [MaxLength(50), Required]
        public string Descricao { get; set; }

        public bool Removido { get; set; }

        public DateTime? RemovidoEm { get; set; }

        public List<ProdutoCategoria> Produtos { get; set; } = new List<ProdutoCategoria>();

        public List<Venda> Vendas { get; set; }
    }
}