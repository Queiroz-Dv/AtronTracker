using Application.DTO;
using Application.DTO.Request;
using Application.Extensions;
using Application.UseCases.EmailCases;
using Application.UseCases.WorkspaceCases;
using Domain.Interfaces.Identity;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Threading.Tasks;

namespace Application.UseCases.UsuarioCases
{
    public sealed class RegistrarContaUsuarioCase(
        IValidador<UsuarioRegistroRequest> Validador,
        VerificarUsuarioExistenteCase VerificarUsuarioExistenteCase,
        ProcessarEnvioEmailConfirmacaoCase ProcessarEnvioEmailConfirmacaoCase,
        RegistrarWorkspaceCase registrarWorkspaceCase,
        IUsuarioIdentityRepository IdentityRepository,
        IUsuarioRepository UsuarioRepository)
    {
        public async Task<Resultado> ExecutarAsync(UsuarioRegistroRequest request)
        {
            var notificacoes = Validador.Validar(request);
            if (notificacoes.TemErros())
                return Resultado.Falha(notificacoes);

            var resultadoVerificacao = await VerificarUsuarioExistenteCase.ExecutarAsync(request.Codigo, request.Email);
            if (resultadoVerificacao.TeveFalha)
                return Resultado.Falha(resultadoVerificacao.Messages);

            if (!await IdentityRepository.RegistrarContaDeUsuarioRepositoryAsync(request.Codigo, request.Email, request.Senha))
                return Resultado.Falha(AuthResource.Erro_GravacaoConta);

            var entidade = request.MapearRequestParaEntidade();
            var gravado = await UsuarioRepository.CriarUsuarioAsync(entidade);
            if (!gravado)
                return Resultado.Falha(UsuarioResource.ErroInesperadoGravacao);

            var usuario = await UsuarioRepository.ObterUsuarioPorCodigoAsync(entidade.Codigo);
            var workspaceDTO = new WorkspaceDTO()
            {
                Codigo = request.Workspace.Codigo,
                Descricao = request.Workspace.Descricao,
                Responsavel = new UsuarioDTO() { 
                    Id = usuario.Id,
                    Codigo = request.Codigo, 
                    Email = request.Email },
            };

            var workspaceResultado = await registrarWorkspaceCase.ExecutarAsync(workspaceDTO);
            if (workspaceResultado.TeveFalha)
            {
                return Resultado.Falha(workspaceResultado.Messages);
            }
            var processoEnvioResultado = await ProcessarEnvioEmailConfirmacaoCase.ExecutarAsync(usuario);

            return Resultado.Sucesso(AuthResource.Mensagem_UsuarioRegistrado);
        }
    }
}