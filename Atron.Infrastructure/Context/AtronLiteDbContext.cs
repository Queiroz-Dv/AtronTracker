using Atron.Domain.Entities;
using Atron.Infrastructure.Interfaces;
using Atron.Infrastructure.Models;
using LiteDB;
using Microsoft.Extensions.Options;
using System;
using System.IO;

namespace Atron.Infrastructure.Context
{
    public class AtronLiteDbContext : ILiteDbContext, IDisposable
    {
        private readonly LiteDatabase _db;

        public IDataSet<Departamento> Departamentos { get; }
        public IDataSet<Cargo> Cargos { get; }

        public AtronLiteDbContext(IOptions<LiteDbOptions> options)
        {
            var cfg = options.Value;
            var basePath = Path.GetFullPath(cfg.DatabasePath);

            var conn = string.IsNullOrEmpty(cfg.Password)
               ? basePath
               : $"Filename={basePath};Password={cfg.Password};";

            _db = new LiteDatabase(conn);
            Departamentos = new LiteDbSet<Departamento>(_db, "Departamentos");
            Cargos = new LiteDbSet<Cargo>(_db, "Cargos");
            EnsureIndexes();
        }

        public void EnsureIndexes()
        {
            EnsuureDepartamentoIndex();
            EnsureCargoIndex();
        }

        private void EnsureCargoIndex()
        {
            var cargos = _db.GetCollection<Cargo>("Cargos");
            cargos.EnsureIndex(c => c.Codigo, unique: true);
        }

        private void EnsuureDepartamentoIndex()
        {
            var departamentos = _db.GetCollection<Departamento>("Departamentos");
            departamentos.EnsureIndex(dpt => dpt.Codigo, unique: true);
        }

        public ILiteCollection<T> GetCollection<T>(string name) where T : class
        {
            return _db.GetCollection<T>(name);
        }

        public void BeginTransaction() => _db.BeginTrans();
        public void Commit() => _db.Commit();
        public void Rollback() => _db.Rollback();
        public void Dispose() => _db?.Dispose();
    }
}