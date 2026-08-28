using System.Linq;
using System.Threading.Tasks;
using Application.Resources;
using Domain.Enums;
using Domain.Interfaces;
using Shared.Domain.ValueObjects;

namespace Application.Services.EntitiesServices.Empresas;

public sealed class EmpresaResponsavelService(
    UsuarioEmpresaAtualService usuarioAtual,
    IEmpresaRepository repository)
{
    public async Task<Resultado<Domain.Entities.UsuarioEmpresa>> ObterAsync()
    {
        var usuario = await usuarioAtual.ObterAsync();
        if (usuario.TeveFalha)
            return Resultado<Domain.Entities.UsuarioEmpresa>.Falhas(usuario.Messages);

        var vinculo = await repository.ObterVinculoAsync(usuario.Dados!.Id, usuario.Dados.Codigo);
        if (vinculo is null
            || vinculo.Papel != PapelUsuarioEmpresa.Responsavel
            || vinculo.Status != StatusUsuarioEmpresa.Ativo
            || vinculo.Empresa.Status != StatusEmpresa.Ativa)
            return Resultado<Domain.Entities.UsuarioEmpresa>.Falha(EmpresaResource.Erro_ResponsavelNecessario);

        return Resultado<Domain.Entities.UsuarioEmpresa>.Sucesso(vinculo);
    }
}
