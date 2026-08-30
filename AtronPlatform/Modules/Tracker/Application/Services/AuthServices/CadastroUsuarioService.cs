using Application.DTO.Request;
using Application.Extensions;
using Application.Interfaces.Services;
using Application.Records.Usuario;
using Domain.Entities;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services.AuthServices
{
    public class CadastroUsuarioService(CadastroUsuarioContextRecord context) : ICadastroUsuarioService
    {
        private const int ValidadeConfirmacaoEmHoras = 24;
        private const int MaximoTentativasConfirmacao = 5;

        public async Task<Resultado> RegistrarAsync(UsuarioRegistroRequest request)
        {
            var notificacoes = context.Validador.Validar(request);
            if (notificacoes.TemErros())
                return Resultado.Falha(notificacoes);

            var usuarioExistente = await context.UsuarioRepository
                .ObterUsuarioGeralPorCodigoAsync(request.Codigo);
            if (usuarioExistente is not null)
                return Resultado.Falha(UsuarioResource.ErroUsuarioExistente);

            if (await context.UsuarioRepository.VerificarEmailExistenteAsync(request.Email))
                return Resultado.Falha(EmailResource.ErroEmailUtilizado);

            if (await context.IdentityRepository.ContaExisteRepositoryAsync(request.Codigo, request.Email))
                return Resultado.Falha(UsuarioResource.ErroUsuarioExistente);

            if (!await context.IdentityRepository.RegistrarContaDeUsuarioRepositoryAsync(request.Codigo, request.Email, request.Senha))
                return Resultado.Falha(AuthResource.Erro_GravacaoConta);

            var usuario = new Usuario(request.Codigo, request.Nome, request.Sobrenome, request.Email,
                request.DataNascimento?.ToDateTime(TimeOnly.MinValue));

            if (!await context.UsuarioRepository.CriarUsuarioAsync(usuario))
                return Resultado.Falha(UsuarioResource.ErroInesperadoGravacao);

            var usuarioGravado = await context.UsuarioRepository.ObterUsuarioPorCodigoAsync(usuario.Codigo);
            var confirmacao = await CriarConfirmacaoAsync(usuarioGravado.Codigo);
            if (!confirmacao.Gravado)
                return Resultado.Falha(AuthResource.Erro_GerarCodigoConfirmacao);

            var resultado = Resultado.Sucesso(string.Format(AuthResource.Mensagem_UsuarioRegistrado, usuario.Nome, usuario.Sobrenome));
            try
            {
                var confirmacaoDeCadastrao = new ConfirmacaoCadastroEmailParametrosRecord(
                    request.Email, usuario.Nome, confirmacao.Identificador, confirmacao.Link, ValidadeConfirmacaoEmHoras);

                var email = context.EmailCompositor.ComporConfirmacaoCadastro(confirmacaoDeCadastrao);

                if (email.TeveFalha)
                    resultado.AdicionarAviso(string.Join(" | ", email.Messages.Select(m => m.Descricao)));

                if ((await context.EmailService.EnviarAsync(email.Dados)).TeveFalha)
                    resultado.AdicionarAviso(AuthResource.Aviso_CadastroCriadoEmailNaoEnviado);
            }
            catch
            {
                resultado.AdicionarAviso(AuthResource.Aviso_CadastroCriadoEmailNaoEnviado);
            }
            return resultado;
        }

        public async Task<Resultado> ConfirmarEmailAsync(string codigoUsuario, string identificador)
        {
            var codigo = codigoUsuario;
            var id = identificador;

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
                    var email = context.EmailCompositor.ComporConfirmacaoConcluida(usuario.Email, usuario.Nome);
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

        private async Task<(string Link, string Identificador, bool Gravado)> CriarConfirmacaoAsync(string codigo)
        {
            var dados = context.ConfirmacaoCodigoService.CriarDadosConfirmacao(codigo, ValidadeConfirmacaoEmHoras);
            var gravado = await context.ConfirmacaoRepository.GravarOuSubstituirAsync(dados.ConfirmacaoEmail);
            var uriBase = context.EnderecoFrontendService.ObterUriBase();
            return ($"{uriBase}/confirmar-email?usuarioCodigo={codigo}", dados.Identificador, gravado);
        }
    }
}
