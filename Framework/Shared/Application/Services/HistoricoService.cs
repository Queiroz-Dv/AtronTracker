using Shared.Application.Interfaces.Repositories;
using Shared.Application.Interfaces.Service;
using Shared.Domain.Entities;
using Shared.Domain.ValueObjects;
using Shared.Extensions;

namespace Shared.Application.Services
{
    public class HistoricoService : IHistoricoService
    {
        private readonly IHistoricoRepository _repository;

        public HistoricoService(IHistoricoRepository repository)
        {
            _repository = repository;
        }
     

        public async Task<Resultado<IList<Historico>>> ObterHistoricoPorContextoCodigoRegistro(string contexto, string codigoRegistro)
        {
            if (codigoRegistro.IsNullOrEmpty())
            {
                return (Resultado<IList<Historico>>)Resultado.Falha("Código do registro vazio ou nulo");
            }
            
            var historicos = await _repository.ListarPorContextoCodigoAsync(contexto, codigoRegistro);

            return Resultado.Sucesso<IList<Historico>>(historicos.ToList());            
        }

        public async Task<Resultado> RegistrarHistoricoAsync(string codigoRegistro, string descricao)
        {
            try
            {
                var historico = new Historico
                {
                    CodigoRegistro = codigoRegistro,
                    Descricao = descricao,
                    DataCriacao = DateTime.Now
                };

                await _repository.AdicionarAsync(historico);
                return Resultado.Sucesso();
            }
            catch (Exception ex)
            {
                return Resultado.Falha($"Erro ao registrar histórico: {ex.Message}");
            }
        }

        public async Task<Resultado> RegistrarHistoricoAsync(Historico historico)
        {
            if (historico == null)
            {
                return Resultado.Falha("Histórico enviado nnão pode ser nulo");
            }

            await _repository.AdicionarAsync(historico);
            return Resultado.Sucesso();
        }
    }
}
