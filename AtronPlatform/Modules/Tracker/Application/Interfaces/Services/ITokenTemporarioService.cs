namespace Application.Interfaces.Services;

public interface ITokenTemporarioService
{
    TokenTemporario Criar();

    string ObterHash(string token);
}

public sealed record TokenTemporario(string Valor, string Hash);
