using System.Linq;
using System.Threading.Tasks;
using Application.Resources;
using Domain.Enums;
using Domain.Interfaces;
using Shared.Application.DTOS.Empresas;
using Shared.Application.Interfaces.Service;

namespace Application.Services.EntitiesServices.Empresas;

public sealed class EmpresaAtualService(
    UsuarioEmpresaAtualService usuarioAtual,
    IEmpresaRepository repository) : IEmpresaAtualService
{
    public async Task<ContextoEmpresa> ObterAsync()
    {
        var resultado = await usuarioAtual.ObterAsync();
        if (resultado.TeveFalha)
            return new(null, null, null, false, resultado.Messages.First().Descricao);

        var usuario = resultado.Dados!;
        var vinculo = await repository.ObterVinculoAsync(usuario.Id, usuario.Codigo);
        if (vinculo is null)
            return new(null, null, null, false, EmpresaResource.Erro_UsuarioSemEmpresa);

        var permitido = vinculo.Status == StatusUsuarioEmpresa.Ativo
            && vinculo.Empresa.Status == StatusEmpresa.Ativa;

        return new(vinculo.EmpresaId, vinculo.Empresa.Codigo, vinculo.Empresa.NomeFantasia,
            permitido, permitido ? null : EmpresaResource.Erro_AcessoIndisponivel);
    }
}
