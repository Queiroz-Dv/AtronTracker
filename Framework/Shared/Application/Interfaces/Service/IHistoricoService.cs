using Shared.Domain.Entities;
using Shared.Domain.ValueObjects;

namespace Shared.Application.Interfaces.Service
{
    public interface IHistoricoService
    {        
        Task<Resultado> RegistrarHistoricoAsync(string codigoRegistro, string descricao);

        Task<Resultado> RegistrarHistoricoAsync(Historico historico);

        Task<Resultado<IList<Historico>>> ObterHistoricoPorCodigoRegistro(string codigoRegistro);

        Task<Resultado<IList<Historico>>> ObterHistoricoPorContextoCodigoRegistro(string contexto, string codigoRegistro);
    }
}