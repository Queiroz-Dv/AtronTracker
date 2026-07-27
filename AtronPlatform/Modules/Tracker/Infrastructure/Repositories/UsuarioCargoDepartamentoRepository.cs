using AtronTracker.Infrastructure.Context;
using Domain.Entities;
using Domain.Interfaces.UsuarioInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UsuarioCargoDepartamentoRepository(AtronDbContext context) :
        Repository<UsuarioCargoDepartamento>(context), IUsuarioCargoDepartamentoRepository
    {
        private readonly AtronDbContext _context = context;

        public async Task<UsuarioCargoDepartamento> ObterPorChaveDoUsuario(int usuarioId, string usuarioCodigo)
        {
            return await _context.UsuarioCargoDepartamentos.FirstOrDefaultAsync(rel => rel.UsuarioId == usuarioId && rel.UsuarioCodigo == usuarioCodigo);
        }

        public async Task<bool> GravarAssociacaoUsuarioCargoDepartamento(Usuario usuario, Cargo cargo, Departamento departamento)
        {
            var usuarioBd = await _context.Usuarios.FirstAsync(usr => usr.Codigo == usuario.Codigo);

            var associacao = new UsuarioCargoDepartamento()
            {
                UsuarioId = usuarioBd.Id,
                UsuarioCodigo = usuario.Codigo,

                DepartamentoId = departamento.Id,
                DepartamentoCodigo = departamento.Codigo,

                CargoId = cargo.Id,
                CargoCodigo = cargo.Codigo
            };

            await _context.UsuarioCargoDepartamentos.AddAsync(associacao);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<UsuarioCargoDepartamento>> ObterPorDepartamento(int id, string codigo)
        {
            return await _context.UsuarioCargoDepartamentos.Where(rel => rel.DepartamentoId == id && rel.DepartamentoCodigo == codigo).ToListAsync();
        }

        public async Task<IEnumerable<UsuarioCargoDepartamento>> ObterPorCargo(int id, string codigo)
        {
            return await _context.UsuarioCargoDepartamentos.Where(rel => rel.CargoId == id && rel.CargoCodigo == codigo).ToListAsync();
        }
    }
}
