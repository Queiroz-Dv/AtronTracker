using Application.Records.Autenticacao;

namespace Application.Interfaces.Services;

public interface ITokenTemporarioService
{
    TokenTemporarioRecord Criar();

    string ObterHash(string token);
}
