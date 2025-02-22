namespace Atron.Domain.Entities
{
    public class Modulo
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Descricao { get; set; }
        public int PerfilDeAcessoId { get; set; }
        public string PerfilDeAcessoCodigo { get; set; }
        public PerfilDeAcesso PerfilDeAcesso { get; set; }
    }
}