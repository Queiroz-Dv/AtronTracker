namespace Shared.Application.DTOS.Common
{
    public interface IDocumento
    {
        string CodigoRegistro { get; set; }

        string Contexto { get; set; }
    }

    public interface IHistoricoDTO : IDocumento
    {
         string Descricao { get; set; }
    }

    public interface IAuditoriaDTO  : IDocumento
    {
         IHistoricoDTO Historico { get; set; }
    }

    public class HistoricoDTO : IHistoricoDTO
    {       
        public string CodigoRegistro { get; set; }
        public string Contexto { get; set; }
        public string Descricao { get; set;}
    }

    public class AuditoriaDTO : IAuditoriaDTO
    {        
        public string CodigoRegistro { get; set; }
        public string Contexto { get; set; }
        public IHistoricoDTO Historico { get; set; }
    }
}
