using System.Threading.Tasks;
using Application.DTO.Response;
using Application.Mapping;
using Application.Resources;
using Application.Services.EntitiesServices.Empresas;
using Domain.Enums;
using Domain.Interfaces;
using Shared.Domain.ValueObjects;

namespace Application.UseCases.EmpresaCases
{
    public sealed class ObterEmpresaCase(
        UsuarioEmpresaAtualService usuarioAtual,
        EmpresaMapping mapping,
        IEmpresaRepository repository)
    {
        public async Task<Resultado<EmpresaResponse?>> ExecutarAsync()
        {
            var usuarioResultado = await usuarioAtual.ObterAsync();
            if (usuarioResultado.TeveFalha)
                return Resultado<EmpresaResponse?>.Falhas(usuarioResultado.Messages);

            var usuario = usuarioResultado.Dados!;
            var vinculo = await repository.ObterVinculoAsync(usuario.Id, usuario.Codigo);
            if (vinculo is null)
                return Resultado<EmpresaResponse?>.Sucesso(null);
            if (vinculo.Status != StatusUsuarioEmpresa.Ativo || vinculo.Empresa.Status != StatusEmpresa.Ativa)
                return Resultado<EmpresaResponse?>.Falha(EmpresaResource.Erro_AcessoIndisponivel);

            return Resultado<EmpresaResponse?>.Sucesso(mapping.MapToDto(vinculo));
        }
    }
}

