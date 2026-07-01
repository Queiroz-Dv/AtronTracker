namespace Shared.Application.DTOS.Common
{
    public class CacheProviderInfoDTO
    {
        public string ProviderConfigurado { get; set; }
        public string ImplementacaoCache { get; set; }
        public string ImplementacaoMemoria { get; set; }
        public string DiretorioArquivoJson { get; set; }
        public bool Distribuido { get; set; }
        public string Observacao { get; set; }
    }
}
