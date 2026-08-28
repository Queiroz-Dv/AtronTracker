using Application.DTO.Request;
using Application.DTO.Response;
using Application.UseCases.EmpresaCases;
using AtronPlatform.WebApi.Security;
using Shared.Application.DTOS.Empresas;
using Shared.Application.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AtronPlatform.WebApi.Controllers.Tracker
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public sealed class EmpresaController(
        CadastrarEmpresaCase cadastrar,
        ObterEmpresaCase obterEmpresa,
        IEmpresaAtualService empresaAtual,
        BuscarEmpresasCase buscarEmpresas,
        SolicitarAssociacaoEmpresaCase solicitarAssociacao,
        ObterSolicitacoesEmpresaCase obterSolicitacoes,
        DecidirSolicitacaoEmpresaCase decidirSolicitacao) : ControllerBase
    {
        [HttpPost]
        [PermitirSemEmpresa]
        public async Task<ActionResult<EmpresaResponse>> Post(EmpresaCadastroRequest request)
        {
            var resultado = await cadastrar.ExecutarAsync(request);
            return resultado.TeveFalha
                ? BadRequest(resultado.Messages)
                : CreatedAtAction(nameof(Get), resultado.Dados);
        }

        [HttpGet]
        [PermitirSemEmpresa]
        public async Task<ActionResult<EmpresaResponse>> Get()
        {
            var resultado = await obterEmpresa.ExecutarAsync();
            if (resultado.TeveFalha)
                return BadRequest(resultado.Messages);
            return resultado.Dados is null ? NoContent() : Ok(resultado.Dados);
        }

        [HttpGet("Contexto")]
        [PermitirSemEmpresa]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<ActionResult<ContextoEmpresa>> Contexto()
            => Ok(await empresaAtual.ObterAsync());

        [HttpGet("Busca")]
        [PermitirSemEmpresa]
        public async Task<ActionResult<IReadOnlyList<EmpresaBuscaResponse>>> Busca([FromQuery] string? termo)
        {
            var resultado = await buscarEmpresas.ExecutarAsync(termo);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Dados);
        }

        [HttpPost("Solicitacoes")]
        [PermitirSemEmpresa]
        public async Task<ActionResult<SolicitacaoEmpresaResponse>> Solicitar(
            SolicitarAssociacaoEmpresaRequest request)
        {
            var resultado = await solicitarAssociacao.ExecutarAsync(request);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Accepted(resultado.Dados);
        }

        [HttpGet("Solicitacoes")]
        public async Task<ActionResult<IReadOnlyList<SolicitacaoEmpresaResponse>>> Solicitacoes()
        {
            var resultado = await obterSolicitacoes.ExecutarAsync();
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Dados);
        }

        [HttpPost("Solicitacoes/{id:int}/Aprovar")]
        public async Task<ActionResult<SolicitacaoEmpresaResponse>> Aprovar(int id)
        {
            var resultado = await decidirSolicitacao.AprovarAsync(id);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Dados);
        }

        [HttpPost("Solicitacoes/{id:int}/Recusar")]
        public async Task<ActionResult<SolicitacaoEmpresaResponse>> Recusar(int id)
        {
            var resultado = await decidirSolicitacao.RecusarAsync(id);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Dados);
        }
    }
}

