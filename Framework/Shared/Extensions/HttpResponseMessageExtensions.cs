using Newtonsoft.Json;
using Shared.DTO;

namespace Shared.Extensions
{
    public static class HttpResponseMessageExtensions
    {
        public static async Task<ValidationErrorResponse> ReadValidationErrorResponseAsync(this HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            var jsonValidation = JsonConvert.DeserializeObject<ValidationErrorResponse>(content);
            return jsonValidation;
        }

        public static async Task<List<ResultResponse>> ReadMessageResponseAsync(this HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            var jsonValidation = JsonConvert.DeserializeObject<List<ResultResponse>>(content);
            return jsonValidation;
        }
    }
}