using Application.DTO.Request;
using Application.DTO.Response;
using Application.Extensions;
using Application.Resources;
using Application.UseCases.WorkspaceCases;
using Domain.Enums;
using Domain.Interfaces.Identity;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Threading.Tasks;

namespace Application.UseCases.UsuarioCases;

public sealed class CadastrarUsuarioCase(
    IValidador<UsuarioRegistroRequest> validador,
    IValidador<CriarWorkspaceInicialRequest> workspaceValidador,
    VerificarUsuarioCase verificarUsuarioCase,
    IUsuarioIdentityRepository usuarioIdentityRepository,
    IUsuarioRepository usuarioRepository,
    CriarWorkspaceInicialCase criarWorkspaceInicialCase,
    CriarConfirmacaoCadastroCase criarConfirmacaoCadastroCase,
    EnviarEmailConfirmacaoCadastroCase enviarEmailConfirmacaoCadastroCase)
{
    public async Task<Resultado> ExecutarAsync(UsuarioRegistroRequest request)
    {
        var notificacoes = validador.Validar(request);
        if (notificacoes.TemErros())
            return Resultado<UsuarioRegistroResponse>.Falhas(notificacoes);

        var verificacao = await verificarUsuarioCase.ExecutarAsync(request.MontarUsuarioRequest());
        if (verificacao.TeveFalha)
            return Resultado<UsuarioRegistroResponse>.Falhas(verificacao.Messages);

        var workspaceRequest = request.Workspace?.MontarWorkspaceInicial(request.Codigo);
        var notificacoesWorkspace = workspaceValidador.Validar(workspaceRequest!);
        if (notificacoesWorkspace.TemErros())
            return Resultado<UsuarioRegistroResponse>.Falhas(notificacoesWorkspace);

        if (workspaceRequest!.Tipo != TipoWorkspace.Pessoal)
            return Resultado<UsuarioRegistroResponse>.Falha(WorkspaceResource.Erro_CadastroTipoNaoDisponivel);

        if (!await usuarioIdentityRepository.RegistrarContaDeUsuarioRepositoryAsync(request.Codigo, request.Email, request.Senha))
            return Resultado<UsuarioRegistroResponse>.Falha(AuthResource.Erro_GravacaoConta);

        var usuario = request.MontarUsuario();
        if (!await usuarioRepository.CriarUsuarioAsync(usuario))
            return Resultado<UsuarioRegistroResponse>.Falha(UsuarioResource.ErroInesperadoGravacao);

        var workspace = await criarWorkspaceInicialCase.ExecutarAsync(workspaceRequest);
        if (workspace.TeveFalha)
            return Resultado<UsuarioRegistroResponse>.Falhas(workspace.Messages);

        var confirmacao = await criarConfirmacaoCadastroCase.ExecutarAsync(usuario.Codigo);
        if (confirmacao.TeveFalha)
            return Resultado<UsuarioRegistroResponse>.Falhas(confirmacao.Messages);


        var envio = await enviarEmailConfirmacaoCadastroCase.ExecutarAsync(usuario, confirmacao.Dados!);

        return Resultado.Sucesso(envio).ComMensagemRegistroSalvo(UsuarioResource.Mensagem_UsuarioSalvo);
    }
}