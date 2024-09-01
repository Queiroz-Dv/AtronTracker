using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Shared.Enums;

namespace Shared.DTO
{
    /// <summary>
    /// DTO base para manipulação dos resultados das respostas da API
    /// </summary>
    [Serializable]
    public class ResultResponseDTO
    {        
        /// <summary>
        /// Mensagem que será apresentada ao usuário
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Nível da mensagem
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public ResultResponseLevelEnum Level {get; set;}
    }
}