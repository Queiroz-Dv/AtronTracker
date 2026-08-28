using Application.Resources;
using Application.Validador;
using Domain.Entities;
using Domain.Interfaces;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Threading.Tasks;

namespace Application.Services.EntitiesServices.Empresas
{
    public sealed class UsuarioEmpresaAtualService(
        IUserAccessor userAccessor,
        IEmpresaRepository repository,
        EmpresaCadastroValidador validador)
    {
        public async Task<Resultado<Usuario>> ObterAsync()
        {
            var codigo = userAccessor.ObterCodigoUsuarioLogado();
            if (string.IsNullOrWhiteSpace(codigo))
                return Resultado<Usuario>.Falha(EmpresaResource.Erro_UsuarioNaoIdentificado);

            var usuario = await repository.ObterUsuarioAsync(codigo);
            var erros = validador.ValidarResponsavel(usuario);
            return erros.TemErros()
                ? Resultado<Usuario>.Falhas(erros)
                : Resultado<Usuario>.Sucesso(usuario!);
        }
    }
}