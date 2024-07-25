namespace Shared
{
    public interface ITransactionResponseClient
    {
        List<Dictionary<string, object>> ResultApiJson { get; }
    }
}