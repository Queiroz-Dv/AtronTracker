using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTO.Response;
using Application.Services.EntitiesServices.Empresas;
using Domain.Interfaces;
using Shared.Domain.ValueObjects;

namespace Application.UseCases.EmpresaCases;

public sealed class ObterSolicitacoesEmpresaCase(
    EmpresaResponsavelService responsavel,
    IEmpresaRepository repository)
{
    public async Task<Resultado<IReadOnlyList<SolicitacaoEmpresaResponse>>> ExecutarAsync()
    {
        var resultado = await responsavel.ObterAsync();
        if (resultado.TeveFalha)
            return Resultado<IReadOnlyList<SolicitacaoEmpresaResponse>>.Falhas(resultado.Messages);

        var itens = await repository.ObterSolicitacoesPendentesAsync(resultado.Dados!.EmpresaId);
        return Resultado<IReadOnlyList<SolicitacaoEmpresaResponse>>.Sucesso(
            itens.Select(Mapear).ToArray());
    }

    private static SolicitacaoEmpresaResponse Mapear(Domain.Entities.SolicitacaoEmpresa item)
        => new(item.Id, item.EmpresaId, item.Empresa.Codigo, item.Empresa.NomeFantasia, item.Status, item.CriadaEm);
}
