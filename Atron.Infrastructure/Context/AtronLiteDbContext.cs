using Atron.Domain.Entities;
using Atron.Infrastructure.Interfaces;
using Atron.Infrastructure.Models;
using LiteDB;
using Microsoft.Extensions.Options;
using Shared.Models.ApplicationModels;
using System.IO;

namespace Atron.Infrastructure.Context
{
    public class LiteUnitOfWork : ILiteUnitOfWork
    {
        private readonly LiteDatabase _database;

        public LiteUnitOfWork(LiteDatabase database)
        {
            _database = database;
        }

        public void BeginTransaction() => _database.BeginTrans();
        public void Commit() => _database.Commit();
        public void Rollback() => _database.Rollback();
        public void Dispose() => _database?.Dispose();
    }

    public class LiteDataSetContext
    {
        public LiteDatabase _db;

        public IDataSet<Departamento> Departamentos { get; }

        public IDataSet<Cargo> Cargos { get; }

        public IDataSet<Usuario> Usuarios { get; }

        public IDataSet<ApplicationUser> Users { get; }

        public IDataSet<UsuarioCargoDepartamento> UsuarioCargoDepartamentos { get; }

        public LiteDataSetContext(IOptions<LiteDbOptions> options)
        {
            var cfg = options.Value;
            var basePath = Path.GetFullPath(cfg.DatabasePath);

            var conn = string.IsNullOrEmpty(cfg.Password)
               ? basePath
               : $"Filename={basePath};Password={cfg.Password};";

            _db = new LiteDatabase(conn);

            Departamentos = new LiteDbSet<Departamento>(_db, "Departamentos");
            Cargos = new LiteDbSet<Cargo>(_db, "Cargos");
            Usuarios = new LiteDbSet<Usuario>(_db, "Usuarios");
            Users = new LiteDbSet<ApplicationUser>(_db, "AppUsers");
            UsuarioCargoDepartamentos = new LiteDbSet<UsuarioCargoDepartamento>(_db, "UsuarioCargoDepartamentos");
        }
    }

    public class AtronLiteDbContext : LiteDataSetContext, ILiteDbContext
    {
        public AtronLiteDbContext(IOptions<LiteDbOptions> options) : base(options)
        {
            EnsureIndexes();
        }

        public void EnsureIndexes()
        {
            EnsureDepartamentoIndex();
            EnsureCargoIndex();
        }

        private void EnsureCargoIndex()
        {
            var cargos = GetCollection<Cargo>("Cargos");
            cargos.EnsureIndex(c => c.Codigo, unique: true);
        }

        private void EnsureDepartamentoIndex()
        {
            var departamentos = GetCollection<Departamento>("Departamentos");
            departamentos.EnsureIndex(dpt => dpt.Codigo, unique: true);
        }

        public ILiteCollection<T> GetCollection<T>(string name) where T : class
        {
            return _db.GetCollection<T>(name);
        }     
    }
}