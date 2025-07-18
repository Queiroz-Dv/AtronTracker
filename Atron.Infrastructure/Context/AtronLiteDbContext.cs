using Atron.Domain.Entities;
using Atron.Infrastructure.Interfaces;
using LiteDB;
using Shared.Extensions;
using Shared.Models.ApplicationModels;
using System.Collections.Generic;
using System.Linq;

namespace Atron.Infrastructure.Context
{
    public class AtronLiteDbContext : LiteDataSetContext
    {
        public LiteDatabase _db;

        public AtronLiteDbContext(LiteDatabase db)
        {
            _db = db;

            Departamentos = new LiteDbSet<Departamento>(_db, "Departamentos");
            Cargos = new LiteDbSet<Cargo>(_db, "Cargos");
            Usuarios = new LiteDbSet<Usuario>(_db, "Usuarios");
            UsuarioIdentity = new LiteDbSet<UsuarioIdentity>(_db, "AppUsers");
            UsuarioCargoDepartamentos = new LiteDbSet<UsuarioCargoDepartamento>(_db, "UsuarioCargoDepartamentos");
            Modulos = new LiteDbSet<Modulo>(_db, "Modulos");
            PerfisDeAcesso = new LiteDbSet<PerfilDeAcesso>(_db, "PerfisDeAcesso");

            EnsureIndexes();
        }

        public void EnsureIndexes()
        {
            EnsureDepartamentoIndex();
            EnsureCargoIndex();
            EnsudereUsuarioIdentityIndexes();
            EnsureUsuarioIndexes();
            EnsureModuloIndexes();
        }

        private void EnsureModuloIndexes()
        {
            var modulos = GetCollection<Modulo>("Modulos");
            modulos.EnsureIndex(m => m.Codigo, unique: true);

            var tipos = new[]
                         {
                            typeof(Departamento),
                            typeof(Cargo),
                            typeof(Usuario),
                            typeof(Salario),
                            typeof(Tarefa),
                            typeof(PerfilDeAcesso),
                            typeof(PerfilDeAcessoUsuario)
                        };

            var modulosInit = tipos
                .Select(t => t.ObterInfoModulo())
                .Where(info => info.HasValue)
                .Select(info => new Modulo
                {
                    Codigo = info.Value.Codigo,
                    Descricao = info.Value.Descricao
                })
                .ToList();

            modulos.InsertBulk(modulosInit);

        }

        private void EnsureUsuarioIndexes()
        {
            var usuarios = GetCollection<Usuario>("Usuarios");
            usuarios.EnsureIndex(u => u.Codigo, unique: true);
            usuarios.EnsureIndex(u => u.Email, unique: true);
        }

        private void EnsudereUsuarioIdentityIndexes()
        {
            var identityUsers = GetCollection<UsuarioIdentity>("AppUsers");
            identityUsers.EnsureIndex(u => u.Codigo, unique: true);
            identityUsers.EnsureIndex(u => u.EmailNormalizado, unique: true);
            identityUsers.EnsureIndex(u => u.Email);
            identityUsers.EnsureIndex(u => u.RefreshToken);
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