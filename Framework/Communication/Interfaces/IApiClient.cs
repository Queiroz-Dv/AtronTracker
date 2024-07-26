namespace Communication.Interfaces
{
    public interface IApiClient
    {
        Task<string> GetAsync(string uri);
        Task<string> PostAsync(string uri, string content);
    }
}