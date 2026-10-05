using AtronAuditoria.Application.Interfaces.Repositories;
using AtronAuditoria.Application.Resources;
using Shared.Application.DTOS.Common;
using Shared.Domain.ValueObjects;

namespace AtronAuditoria.Application.UseCases
{
    public class ObterAuditoriaCase
    {
        private readonly IAuditoriaRepository _auditoriaRepository;
        private readonly IHistoricoRepository _historicoRepository;

        public ObterAuditoriaCase(
            IAuditoriaRepository auditoriaRepository,
            IHistoricoRepository historicoRepository)
        {
            _auditoriaRepository = auditoriaRepository;
            _historicoRepository = historicoRepository;
        }

        public async Task<Resultado<AuditoriaResult>> ExecutarAsync(string contexto, string codigoRegistro)
        {
            if (string.IsNullOrWhiteSpace(codigoRegistro) || string.IsNullOrWhiteSpace(contexto))
            {
                return Resultado<AuditoriaResult>.Falha(AuditoriaResource.ErroCodigoOuContextoObrigatorio);
            }

            var auditoria = await _auditoriaRepository.ObterPorContextoCodigoAsync(contexto, codigoRegistro);

            if (auditoria == null)
            {
                return Resultado<AuditoriaResult>.Falha(AuditoriaResource.ErroAuditoriaNaoEncontrada);
            }

            var result = new AuditoriaResult
            {
                Id = auditoria.Id,
                DataCriacao = auditoria.DataCriacao,
                DataAlteracao = auditoria.DataAlteracao,
                CriadoPor = auditoria.CriadoPor,
                AlteradoPor = auditoria.AlteradoPor,
                Historicos = new List<HistoricoResult>()
            };

            var historicos = await _historicoRepository.ListarPorContextoCodigoAsync(contexto, codigoRegistro);
            if (historicos != null)
            {
                foreach (var h in historicos)
                {
                    result.Historicos.Add(new HistoricoResult
                    {
                        Id = h.Id,
                        Descricao = h.Descricao,
                        DataCriacao = h.DataCriacao
                    });
                }
            }

            return Resultado<AuditoriaResult>.Sucesso(result);
        }
    }
}
