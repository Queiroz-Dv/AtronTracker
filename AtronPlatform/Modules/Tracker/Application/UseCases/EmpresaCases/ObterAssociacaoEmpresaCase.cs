using System.Threading.Tasks;
using Application.DTO.Response;
using Application.Services.EntitiesServices.Empresas;
using Domain.Interfaces;
using Shared.Domain.ValueObjects;

namespace Application.UseCases.EmpresaCases;

public sealed class ObterAssociacaoEmpresaCase(
    UsuarioEmpresaAtualService usuarioAtual,
    IEmpresaRepository repository)
{
    public async Task<Resultado<SolicitacaoEmpresaResponse>> ExecutarAsync()
    {
        var usuario = await usuarioAtual.ObterAsync();
        if (usuario.TeveFalha)
            return Resultado<SolicitacaoEmpresaResponse>.Falhas(usuario.Messages);

        var solicitacao = await repository.ObterUltimaSolicitacaoAsync(
            usuario.Dados!.Id, usuario.Dados.Codigo);

        return Resultado<SolicitacaoEmpresaResponse>.Sucesso(
            solicitacao is null ? null : new SolicitacaoEmpresaResponse(
                solicitacao.Id, solicitacao.EmpresaId, solicitacao.Empresa.Codigo,
                solicitacao.Empresa.NomeFantasia, solicitacao.Status, solicitacao.CriadaEm));
    }
}
