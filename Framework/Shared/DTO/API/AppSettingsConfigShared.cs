namespace Shared.DTO.API
{
    /// <summary>
    /// Classe para o fluxo de configurações descritas no AppSettings
    /// </summary>
    public class AppSettingsConfigShared
    {
        /// <summary>
        /// Lista de rotas padrões para conexão com a API
        /// </summary>
        public List<RotasFixasConfig> RotasFixas { get; set; } 
    }
}
