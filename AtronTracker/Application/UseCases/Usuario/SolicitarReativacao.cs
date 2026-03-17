using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.DTOS.Requests;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Threading.Tasks;

namespace Application.UseCases.Usuario
{
    public class SolicitarReativacao
    {
        private readonly IUsuarioRepository _usuarioRepository;        
        private readonly IEmailService _emailService;

        public SolicitarReativacao(
            IUsuarioRepository usuarioRepository,
            IEmailService emailService)
        {
            _usuarioRepository = usuarioRepository;
            _emailService = emailService;
        }

        public async Task<Resultado> ExecutarAsync(string email)
        {
            if (email.IsNullOrEmpty())
                return Resultado.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var usuario = await _usuarioRepository.ObterInativoPorEmailAsync(email);
            if (usuario is null)
                return Resultado.Falha("Usuário não encontrado");

            await EnviarCodigoPorEmailAsync(usuario.Email, usuario.Nome, usuario.CodigoReativacao);

            return Resultado.Sucesso().AdicionarMensagem("Código de reativação enviado para o e-mail cadastrado.");
        }

        private async Task EnviarCodigoPorEmailAsync(string destinatario, string nome, string codigo)
        {
            try
            {
                var corpo = $@"
                    <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <h2>Reativação de Conta — Sistema Atron</h2>
                        <p>Olá, <strong>{nome}</strong>.</p>
                        <p>Seu código de reativação é:</p>
                        <h1 style='letter-spacing: 8px; color: #007bff;'>{codigo}</h1>
                        <p>Informe este código junto ao seu e-mail para reativar sua conta.</p>
                    </div>";

                await _emailService.EnviarAsync(new EmailRequest
                {
                    EmailsDestino = [destinatario],
                    Assunto = "Código de reativação — Atron",
                    Mensagem = corpo
                });
            }
            catch { }
        }
    }
}