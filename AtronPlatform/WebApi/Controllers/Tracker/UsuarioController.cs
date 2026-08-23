using Application.DTO.Request;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Application.Resources;
using Shared.Authorization;
using Shared.Domain.ValueObjects;
using Shared.Infrastructure.Filters;

namespace AtronPlatform.WebApi.Controllers.Tracker
{
    /// <summary>
    /// Controller para operações de gerenciamento de usuários do sistema.
    /// Contém endpoints para criação, atualização, remoção, desativação, reativação e consultas de usuários.
    /// Requer autorização com a policy Modulo:USR para a maioria das operações.
    /// </summary>
    [Authorize(Policy = ModuloPolicies.Usuario)]
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController(IUsuarioService usuarioService) : ControllerBase
    {
        /// <summary>
        /// Cria um novo usuário no sistema.
        /// </summary>
        /// <param name="request">Objeto com os dados do usuário a ser criado.</param>
        /// <returns>200 OK com mensagens de sucesso ou 400 BadRequest com mensagens de erro.</returns>
        [HttpPost]
        [Transactional]
        public async Task<ActionResult> Post([FromBody] UsuarioRequest request)
        {
            var resultado = await usuarioService.CriarAsync(request);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }

        /// <summary>
        /// Atualiza um usuário existente identificado pelo código.
        /// </summary>
        /// <param name="codigo">Código do usuário (identificador único).</param>
        /// <param name="request">Objeto com os dados atualizados do usuário.</param>
        /// <returns>200 OK com mensagens de sucesso ou 400 BadRequest com mensagens de erro.</returns>
        [HttpPut("{codigo}")]
        [Transactional]
        public async Task<ActionResult> Put(string codigo, [FromBody] UsuarioRequest request)
        {
            if (codigo != request.Codigo)
                return BadRequest(Resultado<object>.Falha(NotificacoesPadronizadas.ErroCodigoRotaDivergente).Messages);

            var resultado = await usuarioService.AtualizarAsync(request);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }

        /// <summary>
        /// Remove um usuário do sistema
        /// </summary>
        /// <param name="codigo">Código do usuário a ser removido.</param>
        /// <returns>200 OK com mensagens de sucesso ou 400 BadRequest com mensagens de erro.</returns>
        [HttpDelete("{codigo}")]
        [Transactional]
        public async Task<ActionResult> Delete(string codigo)
        {
            var resultado = await usuarioService.RemoverAsync(codigo);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }

        /// <summary>
        /// Desativa um usuário
        /// </summary>
        /// <param name="codigo">Código do usuário a ser desativado.</param>
        /// <returns>200 OK com mensagens de sucesso ou 400 BadRequest com mensagens de erro.</returns>
        [HttpPut("desativar/{codigo}")]
        [Transactional]
        public async Task<ActionResult> Desativar(string codigo)
        {
            var resultado = await usuarioService.DesativarAsync(codigo);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }

        /// <summary>
        /// Obtém todos os usuários do sistema.
        /// </summary>
        /// <returns>200 OK com a lista de usuários.</returns>
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var resultado = await usuarioService.ObterTodosAsync();
            return Ok(resultado.Dados);
        }

        /// <summary>
        /// Obtém um usuário pelo código.
        /// </summary>
        /// <param name="codigo">Código do usuário a ser consultado.</param>
        /// <returns>200 OK com o DTO do usuário ou 404 NotFound se não encontrado.</returns>
        [HttpGet("{codigo}")]
        public async Task<ActionResult> Get(string codigo)
        {
            var resultado = await usuarioService.ObterPorCodigoAsync(codigo);
            return resultado.TeveFalha ? NotFound(resultado.Messages) : Ok(resultado.Dados);
        }

        /// <summary>
        /// Solicita alteração de e-mail para o usuário informado — envia token de confirmação para o novo e-mail.
        /// </summary>
        /// <param name="codigo">Código do usuário cujo e-mail será alterado.</param>
        /// <param name="request">Objeto contendo o novo e-mail.</param>
        /// <returns>200 OK com dados ou 400 BadRequest com mensagens de erro.</returns>
        [Authorize]
        [HttpPut("alterar-email/{codigo}")]
        public async Task<IActionResult> AlterarEmail(string codigo, [FromBody] AlterarEmailRequest request)
        {
            var resultado = await usuarioService.AlterarEmailAsync(codigo, request.EmailNovo);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Dados);
        }

        /// <summary>
        /// Confirma a alteração de e-mail usando token recebido no novo e-mail.
        /// </summary>
        /// <param name="usuarioCodigo">Código do usuário cuja alteração será confirmada.</param>
        /// <param name="emailNovo">Novo e-mail que está sendo confirmado.</param>
        /// <param name="token">Token de confirmação enviado para o novo e-mail.</param>
        /// <returns>200 OK com dados ou 400 BadRequest com mensagens de erro.</returns>
        [AllowAnonymous]
        [HttpGet("confirmar-alteracao-email")]
        public async Task<IActionResult> ConfirmarAlteracaoEmail(
            [FromQuery] string usuarioCodigo,
            [FromQuery] string emailNovo,
            [FromQuery] string token)
        {
            var resultado = await usuarioService.ConfirmarAlteracaoEmailAsync(usuarioCodigo, emailNovo, token);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Dados);
        }

        /// <summary>
        /// Reenvia o e-mail de confirmação de cadastro para o usuário autenticado.
        /// </summary>
        /// <param name="codigo">Código do usuário que solicita o reenvio.</param>
        /// <returns>200 OK com dados ou 400 BadRequest com mensagens de erro.</returns>
        [Authorize]
        [HttpPost("reenviar-confirmacao-email/{codigo}")]
        public async Task<IActionResult> ReenviarConfirmacaoEmail(string codigo)
        {
            var resultado = await usuarioService.ReenviarConfirmacaoEmailAsync(codigo);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Dados);
        }
    }
}
