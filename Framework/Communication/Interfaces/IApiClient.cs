namespace Communication.Interfaces
{
    public interface IApiClient
    {
        Task DeleteAsync(string uri, string codigo);
        Task<string> GetAsync(string uri);
        Task PostAsync(string uri, string content);
        Task PutAsync(string uri, string parameter, string content);
    }
}