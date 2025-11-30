using Shared.Application.DTOS.Common;
using Shared.Application.Interfaces.Repositories;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
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
     
        public async Task<Resultado<IList<Historico>>> ObterPorChaveServiceAsync(IHistoricoDTO historicoDTO)
        {
            if (historicoDTO.CodigoRegistro.IsNullOrEmpty())
            {
                return Resultado.Falha<IList<Historico>>(HistoricoResource.ErroCodigoNulo);
            }
            
            var historicos = await _repository.ListarPorContextoCodigoAsync(historicoDTO.Contexto, historicoDTO.CodigoRegistro);

            return Resultado.Sucesso<IList<Historico>>(historicos.ToList());
        }

        public async Task<Resultado> RegistrarServiceAsync(IHistoricoDTO historicoDTO)
        {
            try
            {
                var historico = new Historico
                {
                    CodigoRegistro = historicoDTO.CodigoRegistro,
                    Descricao = historicoDTO.Descricao,
                    Contexto = historicoDTO.Contexto,
                    DataCriacao = DateTime.Now
                };

                await _repository.AdicionarAsync(historico);
                return Resultado.Sucesso();
            }
            catch (Exception ex)
            {
                return Resultado.Falha(string.Format(HistoricoResource.ExecaoRegistrarHistorico, ex.Message));
            }
        }        
    }
}