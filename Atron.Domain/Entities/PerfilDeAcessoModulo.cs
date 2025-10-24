namespace Atron.Tracker.Domain.Entities
{
    public class PerfilDeAcessoModulo
    {
        public int PerfilDeAcessoId { get; set; }
        public string PerfilDeAcessoCodigo { get; set; }


        public int ModuloId { get; set; }
        public string ModuloCodigo { get; set; }

        public PerfilDeAcesso PerfilDeAcesso { get; set; }
        public Modulo Modulo { get; set; }
    }
}