using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTO.Response;
using Application.Services.EntitiesServices.Empresas;
using Domain.Interfaces;
using Shared.Domain.ValueObjects;

namespace Application.UseCases.EmpresaCases;

public sealed class BuscarEmpresasCase(
    UsuarioEmpresaAtualService usuarioAtual,
    IEmpresaRepository repository)
{
    public async Task<Resultado<IReadOnlyList<EmpresaBuscaResponse>>> ExecutarAsync(string? termo)
    {
        var usuario = await usuarioAtual.ObterAsync();
        if (usuario.TeveFalha)
            return Resultado<IReadOnlyList<EmpresaBuscaResponse>>.Falhas(usuario.Messages);

        var empresas = await repository.BuscarAtivasAsync(termo);
        return Resultado<IReadOnlyList<EmpresaBuscaResponse>>.Sucesso(
            empresas.Select(empresa => new EmpresaBuscaResponse(
                empresa.Id, empresa.Codigo, empresa.NomeFantasia)).ToArray());
    }
}
