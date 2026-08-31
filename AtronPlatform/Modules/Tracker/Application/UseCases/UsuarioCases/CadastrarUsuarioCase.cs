using Application.DTO.Request;
using Application.DTO.Response;
using Application.Extensions;
using Application.Resources;
using Application.UseCases.EmpresaCases;
using Application.UseCases.WorkspaceCases;
using Domain.Enums;
using Domain.Extensions;
using Domain.Interfaces.Identity;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace Application.UseCases.UsuarioCases;

public sealed class CadastrarUsuarioCase(
    IValidador<UsuarioRegistroRequest> validador,
    IValidador<CriarWorkspaceInicialRequest> workspaceValidador,
    VerificarUsuarioCase verificarUsuarioCase,
    IUsuarioIdentityRepository usuarioIdentityRepository,
    IUsuarioRepository usuarioRepository,
    CriarEmpresaCase criarEmpresaCase,
    CriarWorkspaceInicialCase criarWorkspaceInicialCase,
    ObterConviteWorkspaceCase obterConviteWorkspaceCase,
    AceitarConviteWorkspaceCase aceitarConviteWorkspaceCase,
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

        var possuiConvite = !string.IsNullOrWhiteSpace(request.Convite);
        CriarWorkspaceInicialRequest? workspaceRequest = null;

        if (possuiConvite)
        {
            if (request.Workspace is not null)
            {
                return Resultado<UsuarioRegistroResponse>.Falha(
                    WorkspaceResource.Erro_ConviteComWorkspace);
            }

            var convite = await obterConviteWorkspaceCase.ExecutarAsync(request.Convite!);
            if (convite.TeveFalha)
                return Resultado<UsuarioRegistroResponse>.Falhas(convite.Messages);
        }
        else
        {
            workspaceRequest = request.Workspace?.MontarWorkspaceInicial(request.Codigo);
            var notificacoesWorkspace = workspaceValidador.Validar(workspaceRequest!);
            if (notificacoesWorkspace.TemErros())
                return Resultado<UsuarioRegistroResponse>.Falhas(notificacoesWorkspace);
        }

        if (!possuiConvite && workspaceRequest!.Tipo == TipoWorkspace.Empresa)
        {
            var empresa = await criarEmpresaCase.ExecutarAsync(
                request.Workspace!.Empresa!.MontarEmpresa());

            if (empresa.TeveFalha)
                return Resultado<UsuarioRegistroResponse>.Falhas(empresa.Messages);
        }

        if (!await usuarioIdentityRepository.RegistrarContaDeUsuarioRepositoryAsync(request.Codigo, request.Email, request.Senha))
            return Resultado<UsuarioRegistroResponse>.Falha(AuthResource.Erro_GravacaoConta);

        var usuario = request.MontarUsuario();
        if (!await usuarioRepository.CriarUsuarioAsync(usuario))
            return Resultado<UsuarioRegistroResponse>.Falha(UsuarioResource.ErroInesperadoGravacao);

        var workspace = possuiConvite
            ? await aceitarConviteWorkspaceCase.ExecutarAsync(
                request.Convite!,
                usuario.Codigo)
            : await criarWorkspaceInicialCase.ExecutarAsync(workspaceRequest!);
        if (workspace.TeveFalha)
            return Resultado<UsuarioRegistroResponse>.Falhas(workspace.Messages);

        var confirmacao = await criarConfirmacaoCadastroCase.ExecutarAsync(usuario.Codigo);
        if (confirmacao.TeveFalha)
            return Resultado<UsuarioRegistroResponse>.Falhas(confirmacao.Messages);


        var envio = await enviarEmailConfirmacaoCadastroCase.ExecutarAsync(usuario, confirmacao.Dados!);

        var mensagens = new NotificationBag();
        mensagens.AdicionarMensagem(
            string.Format(AuthResource.Mensagem_UsuarioRegistrado, usuario.ObterNome()));

        if (envio.TeveFalha)
        {
            mensagens.AdicionarAviso(
                string.Join(" | ", envio.Messages.Select(mensagem => mensagem.Descricao)));
        }

        var response = new UsuarioRegistroResponse(
            usuario.Codigo,
            workspace.Dados!,
            mensagens.Messages);

        return Resultado<UsuarioRegistroResponse>.Sucesso(response);
    }
}
