using AtronNotificacoes.Application;
using AtronNotificacoes.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AtronNotificacoes.Controllers;

[ApiController]
[Authorize(Policy = SegurancaNotificacoes.PoliticaPublicador)]
[Route("api/notificacoes/publicacoes")]
public sealed class PublicacaoNotificacoesInternasController(
    INotificacaoInternaService service,
    ILogger<PublicacaoNotificacoesInternasController> logger) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(NotificacaoInternaResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<NotificacaoInternaResponse>> Publicar(PublicarNotificacaoInternaRequest request)
    {
        try
        {
            var notificacao = await service.CriarAsync(request);
            ObservabilidadeNotificacoes.RegistrarPublicacao(request);
            logger.LogInformation(
                "Notificação interna publicada. Módulo: {ModuloOrigem}; evento: {TipoEvento}; correlação: {CorrelacaoId}.",
                request.ModuloOrigem,
                request.TipoEvento,
                request.CorrelacaoId);

            return CreatedAtAction(nameof(Publicar), new { notificacao.Id }, notificacao);
        }
        catch (Exception exception)
        {
            ObservabilidadeNotificacoes.RegistrarFalhaDePublicacao(request);
            logger.LogError(
                exception,
                "Falha ao publicar notificação interna. Módulo: {ModuloOrigem}; evento: {TipoEvento}; correlação: {CorrelacaoId}.",
                request.ModuloOrigem,
                request.TipoEvento,
                request.CorrelacaoId);
            throw;
        }
    }
}
