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

        public AuditoriaService(
            IAuditoriaRepository repository,
            IUserAccessor userAccessor)
        {
            _repository = repository;
            _userAccessor = userAccessor;
        }

        public async Task<Resultado> RegistrarCriacaoAsync(string codigoRegistro, string? usuario = null)
        {
            try
            {                
                string usuarioFinal = !string.IsNullOrEmpty(usuario)
                                      ? usuario
                                      : _userAccessor.ObterUsuario();

                var auditoria = new Auditoria
                {
                    CodigoRegistro = codigoRegistro,
                    CriadoPor = usuarioFinal,
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

        public async Task<Resultado> RegistrarAlteracaoAsync(string codigoRegistro, string? usuario = null)
        {
            try
            {
                string usuarioFinal = !string.IsNullOrEmpty(usuario)
                                      ? usuario
                                      : _userAccessor.ObterUsuario();

                var auditoria = await _repository.ObterPorCodigoRegistroAsync(codigoRegistro);
                if (auditoria == null)
                    return Resultado.Falha($"Auditoria não encontrada para o código {codigoRegistro}");

                auditoria.AlteradoPor = usuarioFinal;
                auditoria.DataAlteracao = DateTime.Now;

                await _repository.AtualizarAsync(auditoria);
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
                string usuarioFinal = !string.IsNullOrEmpty(usuario)
                                      ? usuario
                                      : _userAccessor.ObterUsuario();

                var auditoria = await _repository.ObterPorCodigoRegistroAsync(codigoRegistro);
                if (auditoria == null)
                    return Resultado.Falha($"Auditoria não encontrada para o código {codigoRegistro}");

                auditoria.RemovidoEm = DateTime.Now;
                auditoria.AlteradoPor = usuarioFinal;
                auditoria.DataAlteracao = DateTime.Now;

                await _repository.AtualizarAsync(auditoria);
                return Resultado.Sucesso();
            }
            catch (Exception ex)
            {
                return Resultado.Falha($"Erro ao registrar remoção na auditoria: {ex.Message}");
            }
        }

        public async Task<Resultado<Auditoria>> ObterPorCodigoRegistro(string codigoRegistro)
        {
            if (codigoRegistro == null)
            {
                return Resultado.Falha<Auditoria>("Código inválido");
            }

            var auditoria = await _repository.ObterPorCodigoRegistroAsync(codigoRegistro);          
            return Resultado.Sucesso(auditoria);
        }
    }
}
