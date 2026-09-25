namespace AtronStock.Domain.Entities
{
    public sealed class LoteProduto
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public List<Produto> Produtos { get; set; } = [];
    }
}
