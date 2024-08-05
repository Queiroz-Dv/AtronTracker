namespace Communication.Interfaces
{
    public interface IApiClient
    {
        Task<string> DeleteAsync(string uri, string codigo);
        Task<string> GetAsync(string uri);
        Task<string> PostAsync(string uri, string content);
        Task<string> PutAsync(string uri, string parameter, string content);
    }
}