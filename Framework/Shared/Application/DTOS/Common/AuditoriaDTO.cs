namespace Shared.Application.DTOS.Common
{
    public record AuditoriaDTO
    {
        public AuditoriaDTO(string codigoRegistro, string contexto, HistoricoDTO historico)
        {
            CodigoRegistro = codigoRegistro;
            Contexto = contexto;
            Historico = historico;
            Historicos = [];
        }

        public string CodigoRegistro { get; private set; }

        public string Contexto { get; private set; }

        public HistoricoDTO Historico { get; set; }

        public IList<HistoricoDTO> Historicos { get; private set; }
    }

    public record HistoricoDTO
    {
        public HistoricoDTO(string codigoRegistro, string contexto, string descricaoHistorico)
        {
            CodigoRegistro = codigoRegistro;
            Contexto = contexto;
            DescricaoHistorico = descricaoHistorico;
        }

        public string CodigoRegistro { get; private set; }
        public string Contexto { get; private set; }
        public string DescricaoHistorico { get; private set; }
    }
}
