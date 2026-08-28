using System.Linq;
using System.Threading.Tasks;
using Application.DTO.Request;
using Application.DTO.Response;
using Application.Mapping;
using Application.Resources;
using Application.Services.EntitiesServices.Empresas;
using Application.Validador;
using Domain.Extensions;
using Domain.Interfaces;
using Shared.Domain.ValueObjects;
using Shared.Extensions;

namespace Application.UseCases.EmpresaCases
{
    public sealed class CadastrarEmpresaCase(
        UsuarioEmpresaAtualService usuarioAtual,
        EmpresaCadastroValidador validador,
        EmpresaMapping mapping,
        IEmpresaRepository repository)
    {
        public async Task<Resultado<EmpresaResponse>> ExecutarAsync(EmpresaCadastroRequest request)
        {
            var usuarioResultado = await usuarioAtual.ObterAsync();
            if (usuarioResultado.TeveFalha)
                return Resultado<EmpresaResponse>.Falhas(usuarioResultado.Messages);

            var erros = validador.Validar(request);
            if (erros.TemErros())
                return Resultado<EmpresaResponse>.Falhas(erros);

            var usuario = usuarioResultado.Dados!;
            if (await repository.ObterVinculoAsync(usuario.Id, usuario.Codigo) is not null)
                return Resultado<EmpresaResponse>.Falha(EmpresaResource.Erro_UsuarioJaVinculado);

            if (await repository.CodigoExisteAsync(request.Codigo))
                return Resultado<EmpresaResponse>.Falha(EmpresaResource.Erro_CodigoExistente);

            var empresa = mapping.MapToEntity(request);
            var errosConclusao = validador.ValidarConclusao(empresa);
            if (errosConclusao.TemErros())
                return Resultado<EmpresaResponse>.Falhas(errosConclusao);

            var vinculo = empresa.ConcluirCadastro(usuario);
            await repository.CriarAsync(empresa);
            return Resultado<EmpresaResponse>.Sucesso(mapping.MapToDto(vinculo));
        }
    }
}