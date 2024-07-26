using Shared.DTO;

namespace Shared
{
    public interface ITransactionResponseClient
    {
        List<ResultResponse> ResultApiJson { get; }
    }
}