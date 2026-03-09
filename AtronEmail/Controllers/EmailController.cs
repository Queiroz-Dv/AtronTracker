using Microsoft.AspNetCore.Mvc;
using Shared.Application.DTOS.Requests;
using Shared.Application.DTOS.Responses;
using Shared.Application.Interfaces.Service;

namespace AtronEmail.Controllers
{
    /// <summary>
    /// Controller para serviços de e-mail e diagnóstico.
    /// Endpoints para uso interno - verificação e diagnóstico do serviço de e-mail.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailDiagnosticService _diagnosticService;

        public EmailController(IEmailDiagnosticService diagnosticService)
        {
            _diagnosticService = diagnosticService;
        }

        /// <summary>
        /// Obtém o status atual do serviço de e-mail.
        /// </summary>
        /// <returns>Status resumido do serviço.</returns>
        [HttpGet("status")]
        [ProducesResponseType(typeof(EmailStatusResponse), 200)]
        public async Task<ActionResult<EmailStatusResponse>> ObterStatus()
        {
            var resultado = await _diagnosticService.ObterStatusAsync();
            return Ok(resultado);
        }

        /// <summary>
        /// Verifica se as configurações de e-mail estão válidas.
        /// </summary>
        /// <returns>Status da configuração e detalhes do provedor identificado.</returns>
        [HttpGet("verificar-configuracao")]
        [ProducesResponseType(typeof(EmailStatusResponse), 200)]
        [ProducesResponseType(typeof(EmailStatusResponse), 400)]
        public async Task<ActionResult<EmailStatusResponse>> VerificarConfiguracao()
        {
            var resultado = await _diagnosticService.VerificarConfiguracaoAsync();

            return resultado.Sucesso
                ? Ok(resultado)
                : BadRequest(resultado);
        }

        /// <summary>
        /// Envia um e-mail de diagnóstico para validar a comunicação.
        /// </summary>
        /// <param name="request">Dados do e-mail de diagnóstico.</param>
        /// <returns>Resultado do envio com detalhes da configuração utilizada.</returns>
        [HttpPost("enviar-diagnostico")]
        [ProducesResponseType(typeof(EmailStatusResponse), 200)]
        [ProducesResponseType(typeof(EmailStatusResponse), 400)]
        public async Task<ActionResult<EmailStatusResponse>> EnviarDiagnostico([FromBody] EmailRequest request)
        {
            var resultado = await _diagnosticService.EnviarDiagnosticoAsync(request);

            return resultado.Sucesso
                ? Ok(resultado)
                : BadRequest(resultado);
        }

        /// <summary>
        /// Envia um e-mail de diagnóstico para o próprio e-mail configurado como remetente.
        /// Útil para validar se as credenciais estão funcionando.
        /// </summary>
        /// <returns>Resultado do envio.</returns>
        [HttpPost("auto-diagnostico")]
        [ProducesResponseType(typeof(EmailStatusResponse), 200)]
        [ProducesResponseType(typeof(EmailStatusResponse), 400)]
        public async Task<ActionResult<EmailStatusResponse>> AutoDiagnostico()
        {            
            var configResult = await _diagnosticService.VerificarConfiguracaoAsync();
            if (!configResult.Sucesso)
            {
                return BadRequest(configResult);
            }
            
            var request = new EmailRequest
            {
                Assunto = "[AUTO-DIAGNÓSTICO] Validação de configuração de e-mail",
                Mensagem = "Este e-mail foi enviado automaticamente para validar que as configurações estão funcionando corretamente."
            };

            request.EmailsDestino = new List<string> { configResult.EmailRemetente! };

            var resultado = await _diagnosticService.EnviarDiagnosticoAsync(request);

            return resultado.Sucesso
                ? Ok(resultado)
                : BadRequest(resultado);
        }
    }
}
