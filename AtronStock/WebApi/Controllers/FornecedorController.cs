using AtronStock.Application.DTO.Request;
using AtronStock.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AtronStock.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FornecedorController : ControllerBase
    {
        private readonly IFornecedorService _fornecedorService;

        public FornecedorController(IFornecedorService fornecedorService)
        {
            _fornecedorService = fornecedorService;
        }

        [HttpPost]
        public async Task<ActionResult> RegistrarFornecedor([FromBody] FornecedorRequest request)
        {
            var resultado = await _fornecedorService.RegistrarFornecedorAsync(request);

            return resultado.TeveFalha ? 
                BadRequest(resultado.ObterNotificacoes()) : 
                Ok(resultado.Response);           
        }

        [HttpGet]
        public async Task<IActionResult> ListarFornecedores()
        {
            var fornecedores = await _fornecedorService.ListarFornecedoresAsync();
            return Ok(fornecedores);
        }

        [HttpGet("{codigo}")]
        public async Task<ActionResult<FornecedorRequest>> ObterPorCodigo(string codigo)
        {
            var resultado = await _fornecedorService.ObterFornecedorPorCodigoAsync(codigo);


            return resultado.TeveFalha ? BadRequest(resultado.ObterNotificacoes()) : Ok(resultado.Dado);
        }
    }
}
