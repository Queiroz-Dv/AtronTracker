namespace Shared.Application.Interfaces.Service
{
    public interface IUserAccessor
    {
        string ObterLogadoUsuario();
        string ObterCodigoUsuarioLogado();
        (string CodigoUsuario, string CodigoWorkspace) ObterDadosDoTenant();
    }
}
