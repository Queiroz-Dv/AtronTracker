using System.Threading.Tasks;
using Application.DTO.Request;
using Application.DTO.Response;
using Application.Resources;
using Application.Services.EntitiesServices.Empresas;
using Domain.Extensions;
using Domain.Interfaces;
using Shared.Domain.ValueObjects;

namespace Application.UseCases.EmpresaCases;

public sealed class SolicitarAssociacaoEmpresaCase(
    UsuarioEmpresaAtualService usuarioAtual,
    IEmpresaRepository repository)
{
    public async Task<Resultado<SolicitacaoEmpresaResponse>> ExecutarAsync(
        SolicitarAssociacaoEmpresaRequest request)
    {
        var usuarioResultado = await usuarioAtual.ObterAsync();
        if (usuarioResultado.TeveFalha)
            return Resultado<SolicitacaoEmpresaResponse>.Falhas(usuarioResultado.Messages);

        if (request.EmpresaId <= 0)
            return Resultado<SolicitacaoEmpresaResponse>.Falha(EmpresaResource.Erro_EmpresaObrigatoria);

        var usuario = usuarioResultado.Dados!;
        if (await repository.ObterVinculoAsync(usuario.Id, usuario.Codigo) is not null)
            return Resultado<SolicitacaoEmpresaResponse>.Falha(EmpresaResource.Erro_UsuarioJaVinculado);

        var empresa = await repository.ObterAtivaAsync(request.EmpresaId);
        if (empresa is null)
            return Resultado<SolicitacaoEmpresaResponse>.Falha(EmpresaResource.Erro_EmpresaNaoEncontrada);

        if (await repository.ObterSolicitacaoPendenteAsync(usuario.Id, usuario.Codigo, empresa.Id) is not null)
            return Resultado<SolicitacaoEmpresaResponse>.Falha(EmpresaResource.Erro_SolicitacaoDuplicada);

        var solicitacao = empresa.CriarSolicitacao(usuario);

        await repository.CriarSolicitacaoAsync(solicitacao);
        return Resultado<SolicitacaoEmpresaResponse>.Sucesso(new SolicitacaoEmpresaResponse(
            solicitacao.Id, empresa.Id, empresa.Codigo, empresa.NomeFantasia,
            solicitacao.Status, solicitacao.CriadaEm));
    }
}
