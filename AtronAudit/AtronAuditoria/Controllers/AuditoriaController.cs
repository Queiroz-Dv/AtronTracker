using Microsoft.AspNetCore.Mvc;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;

namespace AtronAuditoria.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuditoriaController : ControllerBase
    {
        private readonly IAuditoriaService _service;
        private readonly IHistoricoService _historicoService;

        public AuditoriaController(IAuditoriaService service, IHistoricoService serviceHistoricoService)
        {
            _service = service;
            _historicoService = serviceHistoricoService;
        }

        [HttpGet("{codigoRegistro}")]
        public async Task<ActionResult<Resultado>> Get(string codigoRegistro)
        {
            var auditoriaResultado = await _service.ObterPorCodigoRegistro(codigoRegistro);
            var historicoResultado = await _historicoService.ObterHistoricoPorCodigoRegistro(codigoRegistro);

            var auditoria = auditoriaResultado.Dado;
            var auditoriaComHistorico = new AuditoriaResponse
            {
                CodigoRegistro = codigoRegistro,
                DataCriacao = auditoria.DataCriacao,
                AlteradoPor = auditoria.AlteradoPor,
                CriadoPor = auditoria.CriadoPor,
                DataAlteracao = auditoria.DataAlteracao,
                RemovidoEm = auditoria.RemovidoEm
            };

            foreach (var item in historicoResultado.Dado)
            {
                if (auditoria.CodigoRegistro == item.CodigoRegistro)
                {
                    var historicoResponse = new HistoricoResponse
                    {
                        CodigoHistorico = item.CodigoHistorico,
                        CodigoRegistro = item.CodigoRegistro,
                        DataCriacao = item.DataCriacao,
                        Descricao = item.Descricao
                    };

                    auditoriaComHistorico.Historico.Add(historicoResponse);
                }
            }

            return Ok(auditoriaComHistorico);
        }
    }

    public class AuditoriaResponse
    {
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public DateTime? DataAlteracao { get; set; }
        public string CriadoPor { get; set; } = string.Empty;
        public string AlteradoPor { get; set; } = string.Empty;
        public string? CodigoRegistro { get; set; }
        public DateTime? RemovidoEm { get; set; }

        public List<HistoricoResponse> Historico { get; set; } = new List<HistoricoResponse>();
    }

    public class HistoricoResponse
    {
        public long CodigoHistorico { get; set; }
        public string? CodigoRegistro { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public string Descricao { get; set; } = string.Empty;
    }
}
