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
    
    public class HistoricoResult
    {
        public int Id { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public string CriadoPor { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; }
    }

    public class AuditoriaResult
    {
        public int Id { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public string CriadoPor { get; set; } = string.Empty;
        public string AlteradoPor { get; set; } = string.Empty;
        public string CodigoRegistro { get; set; } = string.Empty;
        public string Contexto { get; set; } = string.Empty;
        public DateTime? RemovidoEm { get; set; }
        public System.Collections.Generic.List<HistoricoResult> Historicos { get; set; } = new();
    }
}
