using Shared.Domain.Entities;
using Shared.Domain.ValueObjects;

namespace Shared.Application.Interfaces.Service
{
    public interface IHistoricoService
    {        
        Task<Resultado> RegistrarEventoAsync(string codigoRegistro, string descricao);

        Task<Resultado<IList<Historico>>> ObterHistoricoPorCodigoRegistro(string codigoRegistro);
    }
}