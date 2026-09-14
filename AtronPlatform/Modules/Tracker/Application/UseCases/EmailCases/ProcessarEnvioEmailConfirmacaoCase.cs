using Application.DTO;
using Application.EmailCompositor.Compositores;
using Application.UseCases.UsuarioCases;
using Domain.Entities;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using System.Linq;
using System.Threading.Tasks;

namespace Application.UseCases.EmailCases
{
    public sealed class ProcessarEnvioEmailConfirmacaoCase(
        IAcessoEmailCompositor EmailCompositor,
        IEmailService EmailService,
        CriarConfirmacaoEmailCase CriarConfirmacaoEmailCase)
    {
        private readonly IAcessoEmailCompositor _emailCompositor = EmailCompositor;
        private readonly IEmailService _emailService = EmailService;
        public CriarConfirmacaoEmailCase _criarConfirmacaoEmailCase = CriarConfirmacaoEmailCase;

        public async Task<Resultado> ExecutarAsync(Usuario usuario)
        {
            var confirmacaoResultado = await _criarConfirmacaoEmailCase.ExecutarAsync(usuario.Codigo);
            if (confirmacaoResultado.TeveFalha)
                return Resultado.Falha(confirmacaoResultado.Messages);

            var confirmacao = confirmacaoResultado.Dados;

            var resultado = Resultado.Sucesso(string.Format(AuthResource.Mensagem_UsuarioRegistrado, usuario.Nome, usuario.Sobrenome));

            try
            {
                var parametros = new ParametrosEmailDTO(confirmacao.Link, confirmacao.Identificador);
                parametros.VincularDadosDeEnvio(usuario.Email, usuario.Nome, usuario.Codigo);

                var email = _emailCompositor.ComporConfirmacaoCadastro(parametros);

                if (email.TeveFalha)
                {
                    resultado.AdicionarAvisos(email.Messages);
                    return resultado;
                }

                // Refatorar o EmailService depois 
                var envioResultado = await _emailService.EnviarAsync(email.Dados);
                if (envioResultado.TeveFalha)
                {
                    resultado.AdicionarAvisos(envioResultado.Messages);
                    resultado.AdicionarAviso(AuthResource.Aviso_CadastroCriadoEmailNaoEnviado);
                    return Resultado.Sucesso(resultado.Messages);
                }

                return Resultado.Sucesso();
            }
            catch
            {
                resultado.AdicionarAviso(AuthResource.Aviso_CadastroCriadoEmailNaoEnviado);
                return Resultado.Sucesso(resultado.Messages);
            }
        }
    }
}