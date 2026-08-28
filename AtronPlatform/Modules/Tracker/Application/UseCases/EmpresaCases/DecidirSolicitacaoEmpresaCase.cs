using System.Threading.Tasks;
using Application.DTO.Response;
using Application.Interfaces.Services;
using Application.Resources;
using Application.Services.EntitiesServices.Empresas;
using AtronNotificacoes.Contracts.DTO.Request;
using AtronNotificacoes.Contracts.Interfaces;
using AtronNotificacoes.Domain.Enums;
using Domain.Entities;
using Domain.Extensions;
using Domain.Interfaces;
using Shared.Domain.ValueObjects;

namespace Application.UseCases.EmpresaCases;

public sealed class DecidirSolicitacaoEmpresaCase(
    EmpresaResponsavelService responsavel,
    IEmpresaRepository repository,
    INotificacoesInternasPublisher notificacoes,
    ICacheUsuarioService cacheUsuarioService = null)
{
    public async Task<Resultado<SolicitacaoEmpresaResponse>> AprovarAsync(int id)
        => await DecidirAsync(id, true);

    public async Task<Resultado<SolicitacaoEmpresaResponse>> RecusarAsync(int id)
        => await DecidirAsync(id, false);

    private async Task<Resultado<SolicitacaoEmpresaResponse>> DecidirAsync(int id, bool aprovar)
    {
        var responsavelResultado = await responsavel.ObterAsync();
        if (responsavelResultado.TeveFalha)
            return Resultado<SolicitacaoEmpresaResponse>.Falhas(responsavelResultado.Messages);

        var solicitacao = await repository.ObterSolicitacaoPendenteAsync(
            id, responsavelResultado.Dados!.EmpresaId);
        if (solicitacao is null)
            return Resultado<SolicitacaoEmpresaResponse>.Falha(EmpresaResource.Erro_SolicitacaoNaoEncontrada);

        if (!aprovar)
        {
            solicitacao.Recusar();
            await repository.AtualizarSolicitacaoAsync(solicitacao);
            return await FinalizarAsync(solicitacao, false);
        }

        if (await repository.ObterVinculoAsync(solicitacao.UsuarioId, solicitacao.UsuarioCodigo) is not null)
            return Resultado<SolicitacaoEmpresaResponse>.Falha(EmpresaResource.Erro_UsuarioJaVinculado);

        solicitacao.Aprovar();
        await repository.AprovarSolicitacaoAsync(solicitacao, solicitacao.CriarMembro());
        return await FinalizarAsync(solicitacao, true);
    }

    private async Task<Resultado<SolicitacaoEmpresaResponse>> FinalizarAsync(SolicitacaoEmpresa solicitacao, bool aprovar)
    {
        cacheUsuarioService?.RemoverCacheDeAcessoTokenInfo(solicitacao.UsuarioCodigo);
        await notificacoes.PublicarAsync(new PublicarNotificacaoInternaRequest
        {
            DestinatarioCodigo = solicitacao.UsuarioCodigo,
            ModuloOrigem = ENotificacaoModulos.Tracker.ToString(),
            TipoEvento = aprovar
                ? EmpresaResource.Evento_AssociacaoAprovada
                : EmpresaResource.Evento_AssociacaoRecusada,
            Titulo = aprovar
                ? EmpresaResource.Titulo_AssociacaoAprovada
                : EmpresaResource.Titulo_AssociacaoRecusada,
            Mensagem = aprovar
                ? string.Format(EmpresaResource.Mensagem_AssociacaoAprovada, solicitacao.Empresa.NomeFantasia)
                : string.Format(EmpresaResource.Mensagem_AssociacaoRecusada, solicitacao.Empresa.NomeFantasia),
            ReferenciaExterna = solicitacao.Id.ToString(),
            DataCriacao = solicitacao.CriadaEm
        });

        return Resultado<SolicitacaoEmpresaResponse>.Sucesso(new SolicitacaoEmpresaResponse(
            solicitacao.Id, solicitacao.EmpresaId, solicitacao.Empresa.Codigo,
            solicitacao.Empresa.NomeFantasia, solicitacao.Status, solicitacao.CriadaEm));
    }
}
