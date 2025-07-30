using Atron.Domain.Entities;
using LiteDB;
using Shared.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace Atron.Infrastructure.Context
{
    public class AtronLiteDbContext : LiteDataSetContext
    {
        public readonly LiteDatabase _db;

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
            PerfisDeAcessoModulo = new LiteDbSet<PerfilDeAcessoModulo>(_db, "PerfisDeAcessoModulo");
            PerfisDeAcessoUsuario = new LiteDbSet<PerfilDeAcessoUsuario>(_db, "PerfisDeAcessoUsuario");
            Tarefas = new LiteDbSet<Tarefa>(_db, "Tarefas");
            TarefasEstados = new LiteDbSet<TarefaEstado>(_db, "TarefasEstados");
            Salarios = new LiteDbSet<Salario>(_db, "Salarios");
            Meses = new LiteDbSet<Mes>(_db, "Meses");
        }

        public void EnsureIndexes()
        {
            EnsureDepartamentoIndex();
            EnsureCargoIndex();
            EnsudereUsuarioIdentityIndexes();
            EnsureUsuarioIndexes();
            EnsureModuloIndexes();
            EnsurePerfilDeAcessoIndexes();

            InicializarTarefasEstados();
            InicializarMeses();
        }

        private void InicializarMeses()
        {
            var meses = GetCollection<Mes>("Meses");
            var mesesInit = new List<Mes>()
            {
                new Mes() { Id = 1, Descricao = "Janeiro" },
                new Mes() { Id = 2, Descricao = "Fevereiro" },
                new Mes() { Id = 3, Descricao = "Março" },
                new Mes() { Id = 4, Descricao = "Abril" },
                new Mes() { Id = 5, Descricao = "Maio" },
                new Mes() { Id = 6, Descricao = "Junho" },
                new Mes() { Id = 7, Descricao = "Julho" },
                new Mes() { Id = 8, Descricao = "Agosto" },
                new Mes() { Id = 9, Descricao = "Setembro" },
                new Mes() { Id = 10, Descricao = "Outubro" },
                new Mes() { Id = 11, Descricao = "Novembro" },
                new Mes() { Id = 12, Descricao = "Dezembro" }
            };

            var mesesBd = meses.FindAll().ToList();
            foreach (var mes in mesesInit)
            {
                if (!mesesBd.Any(m => m.Id == mes.Id))
                {
                    meses.Insert(mes);
                }
            }
        }

        private void InicializarTarefasEstados()
        {
            var tarefasEstados = GetCollection<TarefaEstado>("TarefasEstados");
            var estadosInit = new List<TarefaEstado>()
            {
                new TarefaEstado() { Descricao = "Pendente" },
                new TarefaEstado() { Descricao = "Em Andamento" },
                new TarefaEstado() { Descricao = "Concluída" },
                new TarefaEstado() { Descricao = "Aguardando Aprovação" },
                new TarefaEstado() { Descricao = "Cancelada" }
            };

            var estadosBd = tarefasEstados.FindAll().ToList();

            foreach (var estado in estadosInit)
            {
                if (!estadosBd.Any(e => e.Descricao == estado.Descricao))
                {
                    tarefasEstados.Insert(estado);
                }
            }
        }

        private void EnsurePerfilDeAcessoIndexes()
        {
            var perfis = GetCollection<PerfilDeAcesso>("PerfisDeAcesso");
            perfis.EnsureIndex(p => p.Codigo, unique: true);

            var perfisInit = new List<PerfilDeAcesso>()
            {
                new PerfilDeAcesso(){ Codigo = "PRF-CMM", Descricao = "Perfil Comum"},
                new PerfilDeAcesso(){ Codigo = "PRF-ADMIN", Descricao = "Perfil Geral"}
            };

            var perfisBd = perfis.FindAll().ToList();

            foreach (var perfil in perfisInit)
            {
                if (!perfisBd.Any(p => p.Codigo == perfil.Codigo))
                {
                    perfis.Insert(perfil);
                }
            }
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

            var modulosBd = modulos.FindAll().ToList();

            foreach (var modulo in modulosInit)
            {
                if (!modulosBd.Any(m => m.Codigo == modulo.Codigo))
                {
                    modulos.Insert(modulo);
                }
            }
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