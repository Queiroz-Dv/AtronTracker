using Domain.Entities;
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

        public async Task<Resultado> RegistrarAuditoriaAsync(AuditoriaDTO auditoriaDTO)
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
                    CriadoPor = _userAccessor.ObterUsuario(),
                    DataCriacao = DataAtual,
                    Contexto = auditoriaDTO.Contexto,
                };

                await RegistrarHistorico(auditoriaDTO);

                await _repository.AdicionarAsync(auditoria);
                return Resultado.Sucesso();
            }
            catch (Exception ex)
            {
                return Resultado.Falha(string.Format(AuditoriaResource.ErroAoCriarAuditoria, ex.Message));
            }
        }

        private async Task RegistrarHistorico(AuditoriaDTO auditoriaDTO)
        {
            if (!auditoriaDTO.DescricaoHistorico.IsNullOrEmpty())
            {
                var historico = new Historico()
                {
                    CodigoRegistro = auditoriaDTO.CodigoRegistro,
                    DataCriacao = DataAtual,
                    Descricao = auditoriaDTO.DescricaoHistorico,
                    Contexto = auditoriaDTO.Contexto
                };

                await _historicoService.RegistrarHistoricoAsync(historico);
            }
        }

        private async Task RegistrarHistorico(string codigoRegistro, string historicoDescricao)
        {
            if (!historicoDescricao.IsNullOrEmpty())
            {
                var historico = new Historico()
                {
                    CodigoRegistro = codigoRegistro,
                    DataCriacao = DataAtual,
                    Descricao = historicoDescricao
                };

                await _historicoService.RegistrarHistoricoAsync(historico);
            }
        }        

        public async Task<Resultado> RegistrarAlteracaoAuditoriaAsync(AuditoriaDTO auditoriaDTO)
        {
            if (!auditoriaDTO.CodigoRegistro.IsNullOrEmpty() || auditoriaDTO.CodigoRegistro.IsNullOrEmpty())
            {
                return Resultado.Falha(AuditoriaResource.ErroCodigoRegistro);
            }

            try
            {
                var auditoria = await _repository.ObterPorContextoCodigoAsync(auditoriaDTO.Contexto, auditoriaDTO.Contexto);
                if (auditoria == null)
                {
                    return Resultado.Falha(AuditoriaResource.ErroAuditoriaNaoEncontrada);
                }

                auditoria.AlteradoPor = _userAccessor.ObterUsuario();
                auditoria.DataAlteracao = DateTime.Now.Date;
                
                await _repository.AtualizarAsync(auditoria);
                await RegistrarHistorico(auditoriaDTO);
                return Resultado.Sucesso();
            }
            catch (Exception ex)
            {
                return Resultado.Falha($"Erro ao atualizar auditoria: {ex.Message}");
            }
        }                    

        public async Task<Resultado<Auditoria>> ObterPorCodigoRegistro(string contexto, string codigoRegistro)
        {
            if (codigoRegistro == null)
            {
                return Resultado.Falha<Auditoria>(AuditoriaResource.ErroCodigoRegistro);
            }

            var auditoria = await _repository.ObterPorContextoCodigoAsync(contexto, codigoRegistro);

            var historicos = await _historicoService.ObterHistoricoPorCodigoRegistro(codigoRegistro);

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

                _ = auditoria.Historicos.OrderBy(static hst => hst.DataCriacao);
            }

            return Resultado.Sucesso(auditoria);
        }

        public async Task<Resultado> RegistrarRemocaoAsync(string codigoRegistro, string usuarioLogado = null, string historicoDescricao = null)
        {
            try
            {

                var auditoria = new Auditoria();
                if (auditoria == null)
                    return Resultado.Falha($"Auditoria não encontrada para o código {codigoRegistro}");

                auditoria.RemovidoEm = DateTime.Now;
                auditoria.AlteradoPor = ObterUsuario(usuarioLogado);
                auditoria.DataAlteracao = DateTime.Now;

                await _repository.AtualizarAsync(auditoria);
                await RegistrarHistorico(codigoRegistro, historicoDescricao);
                return Resultado.Sucesso();
            }
            catch (Exception ex)
            {
                return Resultado.Falha($"Erro ao registrar remoção na auditoria: {ex.Message}");
            }
        }
    }
}
