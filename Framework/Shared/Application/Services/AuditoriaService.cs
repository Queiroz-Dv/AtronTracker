using Microsoft.IdentityModel.Tokens;
using Shared.Application.DTOS.Common;
using Shared.Application.Interfaces.Repositories;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.Entities;
using Shared.Domain.ValueObjects;

namespace Shared.Application.Services
{
    public class AuditoriaService : IAuditoriaService
    {
        private readonly IAuditoriaRepository _repository;
        private readonly IUserAccessor _userAccessor;
        private readonly IHistoricoService _historicoService;
        private readonly DateTime DataAtual;

        public AuditoriaService(
            IAuditoriaRepository repository,
            IUserAccessor userAccessor,
            IHistoricoService historicoService)
        {
            _repository = repository;
            _userAccessor = userAccessor;
            _historicoService = historicoService;
            DataAtual = DateTime.Now;
        }

        public async Task<Resultado> RegistrarServiceAsync(IAuditoriaDTO auditoriaDTO)
        {
            if (auditoriaDTO.CodigoRegistro.IsNullOrEmpty())
            {
                return Resultado.Falha(AuditoriaResource.ErroCodigoRegistro);
            }

            try
            {
                var auditoria = new Auditoria
                {
                    CodigoRegistro = auditoriaDTO.CodigoRegistro,
                    CriadoPor = _userAccessor.ObterLogadoUsuario(),
                    DataCriacao = DataAtual,
                    Contexto = auditoriaDTO.Contexto
                };

                await RegistrarHistorico(auditoriaDTO.Historico);

                await _repository.AdicionarAsync(auditoria);
                return Resultado.Sucesso();
            }
            catch (Exception ex)
            {
                return Resultado.Falha(string.Format(AuditoriaResource.ErroAoCriarAuditoria, ex.Message));
            }
        }

        private async Task RegistrarHistorico(IHistoricoDTO historicoDTO)
        {
            if (!historicoDTO.Descricao.IsNullOrEmpty())
            {
                await _historicoService.RegistrarServiceAsync(historicoDTO);
            }
        }

        public async Task<Resultado> AtualizarServiceAsync(IAuditoriaDTO auditoriaDTO)
        {
            if (auditoriaDTO.CodigoRegistro.IsNullOrEmpty() || auditoriaDTO.Contexto.IsNullOrEmpty())
            {
                return Resultado.Falha(AuditoriaResource.ErroCodigoRegistro);
            }

            try
            {
                var auditoria = await _repository.ObterPorContextoCodigoAsync(auditoriaDTO.Contexto, auditoriaDTO.CodigoRegistro);
                if (auditoria == null)
                {
                    return Resultado.Falha(AuditoriaResource.ErroAuditoriaNaoEncontrada);
                }

                auditoria.AlteradoPor = _userAccessor.ObterLogadoUsuario();
                auditoria.DataAlteracao = DateTime.Now.Date;

                await _repository.AtualizarAsync(auditoria);
                await RegistrarHistorico(auditoriaDTO.Historico);
                return Resultado.Sucesso();
            }
            catch (Exception ex)
            {
                return Resultado.Falha(string.Format(AuditoriaResource.ErroAoCriarAuditoria, ex.Message));
            }
        }

        public async Task<Resultado<Auditoria>> ObterPorChaveServiceAsync(IAuditoriaDTO auditoriaDTO)
        {
            if (auditoriaDTO.CodigoRegistro == null)
            {
                return Resultado.Falha<Auditoria>(AuditoriaResource.ErroCodigoRegistro);
            }

            Auditoria auditoria = await _repository.ObterPorContextoCodigoAsync(auditoriaDTO.Contexto, auditoriaDTO.CodigoRegistro);

            if (auditoria == null)
            {
                return Resultado.Falha<Auditoria>(AuditoriaResource.ErroAuditoriaNaoEncontrada);
            }


            var historicos = await _historicoService.ObterPorChaveServiceAsync(new HistoricoDTO() { CodigoRegistro = auditoria.CodigoRegistro, Contexto = auditoria.Contexto });
            if (historicos.Dado != null)
            {
                foreach (var item in historicos.Dado)
                {
                    var historicoResponse = new Historico
                    {
                        CodigoHistorico = item.CodigoHistorico,
                        CodigoRegistro = item.CodigoRegistro,
                        DataCriacao = item.DataCriacao,
                        Descricao = item.Descricao
                    };

                    auditoria.Historicos.Add(historicoResponse);
                }
            }

            return Resultado.Sucesso(auditoria);
        }

        public async Task<Resultado> RemoverServiceAsync(IAuditoriaDTO auditoriaDTO)
        {
            try
            {
                var auditoria = await _repository.ObterPorContextoCodigoAsync(auditoriaDTO.Contexto, auditoriaDTO.CodigoRegistro);
                if (auditoria == null)
                {
                    return Resultado.Falha<Auditoria>(AuditoriaResource.ErroAuditoriaNaoEncontrada);
                }

                auditoria.RemovidoEm = DateTime.Now;
                auditoria.AlteradoPor = _userAccessor.ObterLogadoUsuario();
                auditoria.DataAlteracao = DateTime.Now;

                await _repository.AtualizarAsync(auditoria);

                await RegistrarHistorico(auditoriaDTO.Historico);
                return Resultado.Sucesso();
            }
            catch (Exception ex)
            {
                return Resultado.Falha(string.Format(AuditoriaResource.ErroAoCriarAuditoria, ex.Message));
            }
        }
    }
}
