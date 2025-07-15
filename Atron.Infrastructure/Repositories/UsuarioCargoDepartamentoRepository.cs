using Atron.Domain.Entities;
using Atron.Domain.Interfaces.UsuarioInterfaces;
using Atron.Infrastructure.Context;
using Atron.Infrastructure.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories
{
    public class UsuarioCargoDepartamentoRepository : Repository<UsuarioCargoDepartamento>, IUsuarioCargoDepartamentoRepository
    {
        public readonly ILiteUnitOfWork _uow;

        public UsuarioCargoDepartamentoRepository(AtronDbContext context,
                                                  ILiteDbContext liteDbContext,
                                                  ILiteUnitOfWork uow) : base(context, liteDbContext)
        {
            _uow = uow;
        }

        public async Task<UsuarioCargoDepartamento> ObterPorChaveDoUsuario(int usuarioId, string usuarioCodigo)
        {
            return await _liteContext.UsuarioCargoDepartamentos.FindOneAsync(rel => rel.UsuarioId == usuarioId && rel.UsuarioCodigo == usuarioCodigo);
        }

        public async Task<bool> GravarAssociacaoUsuarioCargoDepartamento(Usuario usuario, Cargo cargo, Departamento departamento)
        {
            var usuarioBd = await _liteContext.Usuarios.FindOneAsync(usr => usr.Codigo == usuario.Codigo);

            var associacao = new UsuarioCargoDepartamento()
            {
                UsuarioId = usuarioBd.Id,
                UsuarioCodigo = usuario.Codigo,

                DepartamentoId = departamento.Id,
                DepartamentoCodigo = departamento.Codigo,

                CargoId = cargo.Id,
                CargoCodigo = cargo.Codigo
            };

            _uow.BeginTransaction();
            try
            {

                int resultado = await _liteContext.UsuarioCargoDepartamentos.InsertAsync(associacao);
                _uow.Commit();
                return resultado > 0;
            }
            catch
            {
                _uow.Rollback();
                throw;
            }
        }

        public async Task<IEnumerable<UsuarioCargoDepartamento>> ObterPorDepartamento(int id, string codigo)
        {
            return await _liteContext.UsuarioCargoDepartamentos.FindAllAsync(rel => rel.DepartamentoId == id && rel.DepartamentoCodigo == codigo);
        }
    }
}