using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using AtronNotificacoes.Contracts;
using Microsoft.IdentityModel.Tokens;

namespace AtronNotificacoes.Client;

public sealed record NotificacoesInternasServiceOptions(
    Uri BaseAddress,
    string Issuer,
    string Audience,
    string SecretKey,
    string NomeDoServico);

public sealed class NotificacoesInternasHttpPublisher : INotificacoesInternasPublisher
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly HttpClient httpClient;
    private readonly NotificacoesInternasServiceOptions options;

    public NotificacoesInternasHttpPublisher(
        HttpClient httpClient,
        NotificacoesInternasServiceOptions options)
    {
        this.httpClient = httpClient;
        this.options = options;
    }

    public async Task<ResultadoPublicacaoNotificacaoInterna> PublicarAsync(
        PublicarNotificacaoInternaRequest request,
        CancellationToken cancellationToken = default)
    {
        using var mensagem = new HttpRequestMessage(HttpMethod.Post, new Uri(options.BaseAddress, "api/notificacoes/publicacoes"));
        mensagem.Headers.Authorization = new("Bearer", CriarTokenDeServico());

        if (!string.IsNullOrWhiteSpace(request.CorrelacaoId))
            mensagem.Headers.TryAddWithoutValidation("X-Correlation-Id", request.CorrelacaoId);

        mensagem.Content = JsonContent.Create(request, options: JsonOptions);

        try
        {
            using var resposta = await httpClient.SendAsync(mensagem, cancellationToken);
            if (!resposta.IsSuccessStatusCode)
                return ResultadoPublicacaoNotificacaoInterna.Falha("O serviço de notificações recusou a publicação.");

            var notificacao = await resposta.Content.ReadFromJsonAsync<NotificacaoInternaResponse>(JsonOptions, cancellationToken);
            return notificacao is null
                ? ResultadoPublicacaoNotificacaoInterna.Falha("O serviço de notificações retornou uma resposta inválida.")
                : ResultadoPublicacaoNotificacaoInterna.Sucesso(notificacao);
        }
        catch (HttpRequestException)
        {
            return ResultadoPublicacaoNotificacaoInterna.Falha("O serviço de notificações está indisponível.");
        }
    }

    private string CriarTokenDeServico()
    {
        var claims = new[]
        {
            new Claim("tipo_token", "servico"),
            new Claim("escopo", "notificacoes.publicar"),
            new Claim(ClaimTypes.NameIdentifier, options.NomeDoServico)
        };
        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SecretKey));
        var token = new JwtSecurityToken(
            options.Issuer,
            options.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(5),
            signingCredentials: new SigningCredentials(chave, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
