using Newtonsoft.Json;
using Shared.DTO;

namespace Shared.Extensions
{
    /// <summary>
    /// Classe com métodos utilitários para as respostas HTTP
    /// </summary>
    public static class HttpResponseMessageExtensions
    {
        public static async Task<ValidationErrorResponse> ReadValidationErrorResponseAsync(this HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            var jsonValidation = JsonConvert.DeserializeObject<ValidationErrorResponse>(content);
            return jsonValidation;
        }

        public static async Task<List<ResultResponseDTO>> ReadMessageResponseAsync(this HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            var jsonValidation = JsonConvert.DeserializeObject<List<ResultResponseDTO>>(content);
            return jsonValidation;
        }
    }
}