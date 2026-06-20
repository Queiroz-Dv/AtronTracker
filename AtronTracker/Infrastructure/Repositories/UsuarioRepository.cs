using AtronTracker.Infrastructure.Context;
using Domain.Entities;
using Domain.Interfaces.UsuarioInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
    {
        private AtronDbContext _context;

        public UsuarioRepository(AtronDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> CriarUsuarioAsync(Usuario usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> AtualizarUsuarioAsync(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> AtualizarSalario(int usuarioId, int quantidadeTotal)
        {
            var usuario = await _context.Usuarios.FirstAsync(usr => usr.Id == usuarioId);
            usuario.SalarioAtual = quantidadeTotal;
            _context.Update(usuario);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoverUsuarioAsync(Usuario usuario)
        {
            _context.Usuarios.Remove(usuario);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Usuario> ObterUsuarioPorIdAsync(int? id)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(usr => usr.Id == id);
        }

        public async Task<Usuario> ObterUsuarioPorCodigoAsync(string codigo)
        {
            return await _context.Usuarios
                .Include(rel => rel.UsuarioCargoDepartamentos)
                    .ThenInclude(crg => crg.Cargo)
                        .ThenInclude(dpt => dpt.Departamento)
                .AsNoTracking()
                .FirstOrDefaultAsync(usr => usr.Codigo == codigo && !usr.Inativo);
        }

        public async Task<Usuario> ObterUsuarioGeralPorCodigoAsync(string codigo)
        {
            return await _context.Usuarios
                .Include(rel => rel.UsuarioCargoDepartamentos)
                    .ThenInclude(crg => crg.Cargo)
                        .ThenInclude(dpt => dpt.Departamento)
                .AsNoTracking()
                .FirstOrDefaultAsync(usr => usr.Codigo == codigo );
        }

        public async Task<Usuario> ObterInativoPorEmailAsync(string email)
        {
            return await _context.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(usr => usr.Email == email && usr.Inativo);
        }

        public async Task<Usuario> ObterUsuarioGeralPorEmailAsync(string email)
        {
            return await _context.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(usr => usr.Email == email);
        }

        public async Task<IEnumerable<Usuario>> ObterUsuariosAsync()
        {
            return await _context.Usuarios
                .Include(rel => rel.UsuarioCargoDepartamentos)
                    .ThenInclude(crg => crg.Cargo)
                        .ThenInclude(dpt => dpt.Departamento)
                .Where(usr => !usr.Inativo)
                .ToListAsync();
        }

        public async Task<bool> VerificarEmailExistenteAsync(string email)
        {
            return await _context.Usuarios.AnyAsync(u => u.Email == email);
        }

        public async Task<List<UsuarioIdentity>> ObterTodosUsuariosDoIdentity()
        {
            return await (from au in _context.Users
                          join u in _context.Usuarios
                              .Include(r => r.UsuarioCargoDepartamentos)
                                  .ThenInclude(crg => crg.Cargo)
                                      .ThenInclude(dpt => dpt.Departamento)
                          on au.UserName equals u.Codigo
                          where !u.Inativo
                          select new UsuarioIdentity
                          {
                              Codigo = u.Codigo,
                              Nome = u.Nome,
                              Sobrenome = u.Sobrenome,
                              Email = u.Email,
                              Salario = u.Salario,
                              DataNascimento = u.DataNascimento,
                              UsuarioCargoDepartamentos = u.UsuarioCargoDepartamentos
                          })
                          .OrderByDescending(c => c.Codigo)
                          .ToListAsync();
        }
    }
}