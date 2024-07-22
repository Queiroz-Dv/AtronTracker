namespace Shared.DTO
{
    [Serializable]
    public class ApiWebViewMessageResponse : ITransactionResponseClient
    {
        public string Message { get; set; }

        public string Level { get; set; }

        public string ResultApiJson { get; set; }
    }
}