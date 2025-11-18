using System.ComponentModel.DataAnnotations;

namespace AtronStock.Domain.Entities
{
    public sealed class Categoria
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(25), Required]
        public string Codigo { get; set; } = string.Empty;

        [MaxLength(50), Required]
        public string Descricao { get; set; } = string.Empty;

        public EStatus Removido { get; set; }

        public DateTime? RemovidoEm { get; set; }

        //public List<ProdutoCategoria> Produtos { get; set; } = [];

        //public List<Venda> Vendas { get; set; } = [];
    }
}