using Atron.Domain.Entities;
using Atron.Infrastructure.Context;

namespace Atron.Infrastructure.Interfaces
{
    public interface ILiteDbContext 
    {
        IDataSet<Departamento> Departamentos { get; }

        IDataSet<Cargo> Cargos { get; }
    }
}