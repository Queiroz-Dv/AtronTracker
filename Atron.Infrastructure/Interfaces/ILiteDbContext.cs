using Atron.Domain.Entities;
using Atron.Infrastructure.Context;
using System;

namespace Atron.Infrastructure.Interfaces
{
    public interface ILiteDbContext : ILiteTransactions, IDisposable
    {
        IDataSet<Departamento> Departamentos { get; }

        IDataSet<Cargo> Cargos { get; }
    }
}