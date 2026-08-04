using Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Shared.Application.Resources;

namespace Infrastructure.Configuration;

public sealed class EnderecoFrontendService : IEnderecoFrontendService
{
    private readonly string _uriBase;

    public EnderecoFrontendService(IConfiguration configuration)
    {
        var uriConfigurada = configuration["Auth:ClientBaseUri"];
        if (string.IsNullOrWhiteSpace(uriConfigurada))
        {
            var origensPermitidas = configuration
                .GetSection("Cors:AllowedOrigins")
                .Get<string[]>()?
                .Where(origem => !string.IsNullOrWhiteSpace(origem))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray()
                ?? [];

            if (origensPermitidas.Length == 1)
                uriConfigurada = origensPermitidas[0];
        }

        if (!Uri.TryCreate(uriConfigurada, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new InvalidOperationException(AuthResource.Erro_UriFrontendNaoConfigurada);
        }

        _uriBase = uri.GetLeftPart(UriPartial.Authority).TrimEnd('/');
    }

    public string ObterUriBase() => _uriBase;
}
