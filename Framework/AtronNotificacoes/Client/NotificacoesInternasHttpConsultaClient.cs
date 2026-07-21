using System.Net.Http.Json;
using System.Text.Json;
using AtronNotificacoes.Contracts;

namespace AtronNotificacoes.Client;

public interface INotificacoesInternasConsultaClient
{
    Task<ResultadoConsultaNotificacoesInternas<IReadOnlyList<NotificacaoInternaResponse>>> ObterMinhasAsync(
        string tokenDoUsuario,
        CancellationToken cancellationToken = default);

    Task<ResultadoConsultaNotificacoesInternas<NotificacaoInternaResponse>> MarcarComoLidaAsync(
        long id,
        string tokenDoUsuario,
        CancellationToken cancellationToken = default);

    Task<ResultadoConsultaNotificacoesInternas<IReadOnlyList<NotificacaoInternaResponse>>> MarcarTodasComoLidasAsync(
        string tokenDoUsuario,
        CancellationToken cancellationToken = default);

    Task<bool> ExcluirAsync(long id, string tokenDoUsuario, CancellationToken cancellationToken = default);
}

public sealed record ResultadoConsultaNotificacoesInternas<T>(bool Sucesso, T? Dados)
{
    public static ResultadoConsultaNotificacoesInternas<T> ComSucesso(T dados) => new(true, dados);
    public static ResultadoConsultaNotificacoesInternas<T> Falha() => new(false, default);
}

public sealed class NotificacoesInternasHttpConsultaClient(HttpClient httpClient, Uri baseAddress) : INotificacoesInternasConsultaClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public Task<ResultadoConsultaNotificacoesInternas<IReadOnlyList<NotificacaoInternaResponse>>> ObterMinhasAsync(
        string tokenDoUsuario,
        CancellationToken cancellationToken = default) =>
        EnviarAsync<IReadOnlyList<NotificacaoInternaResponse>>(
            HttpMethod.Get,
            "api/notificacoes",
            tokenDoUsuario,
            cancellationToken);

    public Task<ResultadoConsultaNotificacoesInternas<NotificacaoInternaResponse>> MarcarComoLidaAsync(
        long id,
        string tokenDoUsuario,
        CancellationToken cancellationToken = default) =>
        EnviarAsync<NotificacaoInternaResponse>(
            HttpMethod.Post,
            $"api/notificacoes/{id}/marcar-como-lida",
            tokenDoUsuario,
            cancellationToken);

    public Task<ResultadoConsultaNotificacoesInternas<IReadOnlyList<NotificacaoInternaResponse>>> MarcarTodasComoLidasAsync(
        string tokenDoUsuario,
        CancellationToken cancellationToken = default) =>
        EnviarAsync<IReadOnlyList<NotificacaoInternaResponse>>(
            HttpMethod.Post,
            "api/notificacoes/marcar-todas-como-lidas",
            tokenDoUsuario,
            cancellationToken);

    public async Task<bool> ExcluirAsync(long id, string tokenDoUsuario, CancellationToken cancellationToken = default)
    {
        using var request = CriarRequisicao(HttpMethod.Delete, $"api/notificacoes/{id}", tokenDoUsuario);

        try
        {
            using var response = await httpClient.SendAsync(request, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException)
        {
            return false;
        }
    }

    private async Task<ResultadoConsultaNotificacoesInternas<T>> EnviarAsync<T>(
        HttpMethod metodo,
        string caminho,
        string tokenDoUsuario,
        CancellationToken cancellationToken)
    {
        using var request = CriarRequisicao(metodo, caminho, tokenDoUsuario);
        if (metodo == HttpMethod.Post)
            request.Content = JsonContent.Create(new { });

        try
        {
            using var response = await httpClient.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return ResultadoConsultaNotificacoesInternas<T>.Falha();

            var dados = await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken);
            return dados is null
                ? ResultadoConsultaNotificacoesInternas<T>.Falha()
                : ResultadoConsultaNotificacoesInternas<T>.ComSucesso(dados);
        }
        catch (HttpRequestException)
        {
            return ResultadoConsultaNotificacoesInternas<T>.Falha();
        }
    }

    private HttpRequestMessage CriarRequisicao(HttpMethod metodo, string caminho, string tokenDoUsuario)
    {
        var request = new HttpRequestMessage(metodo, new Uri(baseAddress, caminho));
        request.Headers.Authorization = new("Bearer", tokenDoUsuario);
        return request;
    }
}

public sealed class NotificacoesInternasConsultaIndisponivel : INotificacoesInternasConsultaClient
{
    public Task<ResultadoConsultaNotificacoesInternas<IReadOnlyList<NotificacaoInternaResponse>>> ObterMinhasAsync(string tokenDoUsuario, CancellationToken cancellationToken = default) =>
        Task.FromResult(ResultadoConsultaNotificacoesInternas<IReadOnlyList<NotificacaoInternaResponse>>.Falha());

    public Task<ResultadoConsultaNotificacoesInternas<NotificacaoInternaResponse>> MarcarComoLidaAsync(long id, string tokenDoUsuario, CancellationToken cancellationToken = default) =>
        Task.FromResult(ResultadoConsultaNotificacoesInternas<NotificacaoInternaResponse>.Falha());

    public Task<ResultadoConsultaNotificacoesInternas<IReadOnlyList<NotificacaoInternaResponse>>> MarcarTodasComoLidasAsync(string tokenDoUsuario, CancellationToken cancellationToken = default) =>
        Task.FromResult(ResultadoConsultaNotificacoesInternas<IReadOnlyList<NotificacaoInternaResponse>>.Falha());

    public Task<bool> ExcluirAsync(long id, string tokenDoUsuario, CancellationToken cancellationToken = default) =>
        Task.FromResult(false);
}

public sealed class NotificacoesInternasPublisherIndisponivel : INotificacoesInternasPublisher
{
    public Task<ResultadoPublicacaoNotificacaoInterna> PublicarAsync(
        PublicarNotificacaoInternaRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(ResultadoPublicacaoNotificacaoInterna.Falha("A central de notificações não está configurada."));
}
