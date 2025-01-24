namespace Communication.Interfaces.Services
{
    public interface IRouterBuilderService
    {
        public abstract void TransferRouteToApiClient(string url);
    }
}