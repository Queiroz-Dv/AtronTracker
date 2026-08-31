using Application.Interfaces.Services;
using Domain.Interfaces;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Threading.Tasks;

namespace Application.UseCases.UsuarioCases;

public sealed class ConfirmarEmailCase(
    IConfirmacaoEmailRepository confirmacaoEmailRepository,
    IConfirmacaoEmailCodigoService confirmacaoEmailCodigoService,
    IUsuarioRepository usuarioRepository,
    EnviarEmailConfirmacaoConcluidaCase enviarEmailConfirmacaoConcluidaCase)
{
    private const int MaximoTentativasConfirmacao = 5;

    public async Task<Resultado> ExecutarAsync(string usuarioCodigo, string identificador)
    {
        if (usuarioCodigo.IsNullOrEmpty() || identificador.IsNullOrEmpty())
            return Resultado.Falha(AuthResource.Erro_DadosConfirmacaoObrigatorios);

        var confirmacao = await confirmacaoEmailRepository.ObterAtivaPorUsuarioAsync(usuarioCodigo);

        if (confirmacao.IsNullable() || confirmacao.TentativasFalhas >= MaximoTentativasConfirmacao)
            return Resultado.Falha(AuthResource.Erro_FalhaConfirmarEmail);

        if (!confirmacaoEmailCodigoService.ConfirmacaoValida(confirmacao, usuarioCodigo, identificador))
        {
            await confirmacaoEmailRepository.RegistrarTentativaFalhaAsync(confirmacao.Id);
            return Resultado.Falha(AuthResource.Erro_FalhaConfirmarEmail);
        }

        if (!await usuarioRepository.ConfirmarEmailAsync(usuarioCodigo))
            return Resultado.Falha(AuthResource.Erro_FalhaConfirmarEmail);

        await confirmacaoEmailRepository.MarcarConfirmadaAsync(confirmacao.Id);

        var resultado = Resultado.Sucesso(AuthResource.Mensagem_EmailConfirmado);
        var usuario = await usuarioRepository.ObterUsuarioPorCodigoAsync(usuarioCodigo);
        if (usuario.IsNullable())
            return resultado;

        await enviarEmailConfirmacaoConcluidaCase.ExecutarAsync(usuario);

        return resultado;
    }
}
