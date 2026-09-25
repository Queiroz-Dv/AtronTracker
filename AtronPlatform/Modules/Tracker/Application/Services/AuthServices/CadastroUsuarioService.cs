using Application.DTO;
using Application.DTO.Request;
using Application.Extensions;
using Application.Interfaces.Services;
using Application.Records.Facade;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services.AuthServices
{
    public class CadastroUsuarioService(CadastroUsuarioFacadeRecord context) : ICadastroUsuarioService
    {
        private const int MaximoTentativasConfirmacao = 5;

        public async Task<Resultado> RegistrarAsync(UsuarioRegistroRequest request)
        {
            var notificacoes = context.Validador.Validar(request);
            if (notificacoes.TemErros())
                return Resultado.Falha(notificacoes);

            var resultadoVerificacao = await context.VerificarUsuarioExistenteCase.ExecutarAsync(request.Codigo, request.Email);
            if (resultadoVerificacao.TeveFalha)
                return Resultado.Falha(resultadoVerificacao.Messages);

            if (!await context.IdentityRepository.RegistrarContaDeUsuarioRepositoryAsync(request.Codigo, request.Email, request.Senha))
                return Resultado.Falha(AuthResource.Erro_GravacaoConta);

            var entidade = request.MapearRequestParaEntidade();

            if (!await context.UsuarioRepository.CriarUsuarioAsync(entidade))
                return Resultado.Falha(UsuarioResource.ErroInesperadoGravacao);

            var usuario = await context.UsuarioRepository.ObterUsuarioPorCodigoAsync(entidade.Codigo);
            var processoEnvioResultado = await context.ProcessarEnvioEmailConfirmacaoCase.ExecutarAsync(usuario);

            return Resultado.Sucesso(processoEnvioResultado.Messages);
        }

        public async Task<Resultado> ConfirmarEmailAsync(string codigoUsuario, string identificador)
        {
            var codigo = codigoUsuario.NormalizeUserCodeIdentifier();
            var id = identificador.NormalizeIdentifier();

            if (string.IsNullOrWhiteSpace(codigo) || string.IsNullOrWhiteSpace(id))
                return Resultado.Falha(AuthResource.Erro_DadosConfirmacaoObrigatorios);

            var confirmacao = await context.ConfirmacaoRepository.ObterAtivaPorUsuarioAsync(codigo);
            if (confirmacao is null || confirmacao.TentativasFalhas >= MaximoTentativasConfirmacao)
                return Resultado.Falha(AuthResource.Erro_FalhaConfirmarEmail);

            if (!context.ConfirmacaoCodigoService.ConfirmacaoValida(confirmacao, codigo, id))
            {
                await context.ConfirmacaoRepository.RegistrarTentativaFalhaAsync(confirmacao.Id);
                return Resultado.Falha(AuthResource.Erro_FalhaConfirmarEmail);
            }

            if (!await context.UsuarioRepository.ConfirmarEmailAsync(codigo))
                return Resultado.Falha(AuthResource.Erro_FalhaConfirmarEmail);

            await context.ConfirmacaoRepository.MarcarConfirmadaAsync(confirmacao.Id);

            var resultado = Resultado.Sucesso(AuthResource.Mensagem_EmailConfirmado);
            var usuario = await context.UsuarioRepository.ObterUsuarioPorCodigoAsync(codigo);

            if (usuario != null && !string.IsNullOrEmpty(usuario.Email))
            {
                try
                {
                    var parametrosEmailDto = new ParametrosEmailDTO()
                    {
                        Email = usuario.Email,
                        UsuarioNome = usuario.Nome,
                    };

                    var email = context.EmailCompositor.ComporConfirmacaoConcluida(parametrosEmailDto);
                    if (email.TeveFalha)
                        resultado.AdicionarAviso(string.Join(" | ", email.Messages.Select(m => m.Descricao)));

                    if ((await context.EmailService.EnviarAsync(email.Dados)).TeveFalha)
                        resultado.AdicionarAviso(AuthResource.Aviso_ConfirmacaoConcluidaEmailNaoEnviado);
                }
                catch
                {
                    resultado.AdicionarAviso(AuthResource.Aviso_ConfirmacaoConcluidaEmailNaoEnviado);
                }
            }
            return resultado;
        }
    }
}
