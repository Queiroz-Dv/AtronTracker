using Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using Shared.Application.Interfaces.Repositories;
using Shared.Application.Interfaces.Service;
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

        public async Task<Resultado> RegistrarAuditoriaAsync(string codigoRegistro, string usuario = null, string historicoDescricao = null)
        {
            if (codigoRegistro.IsNullOrEmpty())
            {
                return Resultado.Falha($"Código de registro não pode estar vazio para registrar a auditoria.");
            }

            try
            {
                var auditoria = new Auditoria
                {
                    CodigoRegistro = codigoRegistro,
                    CriadoPor = ObterUsuario(usuario),
                    DataCriacao = DataAtual
                };

                await RegistrarHistorico(codigoRegistro, historicoDescricao);

                await _repository.AdicionarAsync(auditoria);
                return Resultado.Sucesso();
            }
            catch (Exception ex)
            {
                return Resultado.Falha($"Erro ao criar auditoria: {ex.Message}");
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

        public async Task<Resultado> RegistrarAuditoriaAsync(string codigoRegistro, string? usuario = null)
        {
            if (!codigoRegistro.IsNullOrEmpty())
            {
                return Resultado.Falha($"Código de registro não pode estar vazio para registrar a auditoria.");
            }

            try
            {                
                var auditoria = new Auditoria
                {
                    CodigoRegistro = codigoRegistro,
                    CriadoPor = ObterUsuario(usuario),
                    DataCriacao = DateTime.Now
                };

                await _repository.AdicionarAsync(auditoria);
                return Resultado.Sucesso();
            }
            catch (Exception ex)
            {
                return Resultado.Falha($"Erro ao criar auditoria: {ex.Message}");
            }
        }

        public async Task<Resultado> RegistrarAlteracaoAuditoriaAsync(string codigoRegistro, string? usuario = null)
        {
            if (!codigoRegistro.IsNullOrEmpty())
            {
                return Resultado.Falha($"Código de registro não pode estar vazio para registrar a auditoria.");
            }

            try
            {
                var auditoria = await _repository.ObterPorCodigoRegistroAsync(codigoRegistro);
                if (auditoria == null)
                {
                    return Resultado.Falha($"Auditoria não encontrada para o código {codigoRegistro}");
                }

                auditoria.AlteradoPor = ObterUsuario(usuario);
                auditoria.DataAlteracao = DateTime.Now;
                
                await _repository.AtualizarAsync(auditoria);
                return Resultado.Sucesso();
            }
            catch (Exception ex)
            {
                return Resultado.Falha($"Erro ao atualizar auditoria: {ex.Message}");
            }
        }

        public async Task<Resultado> RegistrarAlteracaoAuditoriaAsync(string codigoRegistro, string? usuario = null, string historicoDescricao = null)
        {
            if (codigoRegistro.IsNullOrEmpty())
            {
                return Resultado.Falha($"Código de registro não pode estar vazio para registrar a auditoria.");
            }

            try
            {
                var auditoria = await _repository.ObterPorCodigoRegistroAsync(codigoRegistro);
                if (auditoria == null)
                    return Resultado.Falha($"Auditoria não encontrada para o código {codigoRegistro}");

                auditoria.AlteradoPor = ObterUsuario(usuario);
                auditoria.DataAlteracao = DateTime.Now;

                await _repository.AtualizarAsync(auditoria);
                await RegistrarHistorico(codigoRegistro, historicoDescricao);
                return Resultado.Sucesso();
            }
            catch (Exception ex)
            {
                return Resultado.Falha($"Erro ao atualizar auditoria: {ex.Message}");
            }
        }

        public async Task<Resultado> RegistrarRemocaoAsync(string codigoRegistro, string? usuario = null)
        {
            try
            {

                var auditoria = await _repository.ObterPorCodigoRegistroAsync(codigoRegistro);
                if (auditoria == null)
                    return Resultado.Falha($"Auditoria não encontrada para o código {codigoRegistro}");

                auditoria.RemovidoEm = DateTime.Now;
                auditoria.AlteradoPor = ObterUsuario(usuario);
                auditoria.DataAlteracao = DateTime.Now;

                await _repository.AtualizarAsync(auditoria);
                return Resultado.Sucesso();
            }
            catch (Exception ex)
            {
                return Resultado.Falha($"Erro ao registrar remoção na auditoria: {ex.Message}");
            }
        }

        private string ObterUsuario(string usuario = null)
        {
            return !string.IsNullOrEmpty(usuario) ? usuario : _userAccessor.ObterUsuario();
        }

        public async Task<Resultado<Auditoria>> ObterPorCodigoRegistro(string codigoRegistro)
        {
            if (codigoRegistro == null)
            {
                return Resultado.Falha<Auditoria>("Código inválido");
            }

            var auditoria = await _repository.ObterPorCodigoRegistroAsync(codigoRegistro);

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

                var auditoria = await _repository.ObterPorCodigoRegistroAsync(codigoRegistro);
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
