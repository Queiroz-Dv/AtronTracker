using AtronTracker.Infrastructure.Context;
using Domain.Entities;
using Domain.Interfaces.UsuarioInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private AtronDbContext _context;

        public UsuarioRepository(AtronDbContext context)
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
            var usuarioBd = await _context.Usuarios
                .FirstOrDefaultAsync(usr => usr.Id == usuario.Id && usr.Codigo == usuario.Codigo);

            if (usuarioBd is null)
                return false;

            usuarioBd.Nome = usuario.Nome;
            usuarioBd.Sobrenome = usuario.Sobrenome;
            usuarioBd.Email = usuario.Email;
            usuarioBd.DataNascimento = usuario.DataNascimento;
            usuarioBd.ReceberNotificacaoInternaTarefa = usuario.ReceberNotificacaoInternaTarefa;
            usuarioBd.ReceberNotificacaoTarefaPorEmail = usuario.ReceberNotificacaoTarefaPorEmail;
            usuarioBd.CodigoReativacao = usuario.CodigoReativacao;
            usuarioBd.Inativo = usuario.Inativo;
            usuarioBd.GestorImediatoId = usuario.GestorImediatoId;
            usuarioBd.GestorImediatoCodigo = usuario.GestorImediatoCodigo;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ConfirmarEmailAsync(string codigo)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(usr => usr.Codigo == codigo && !usr.Inativo);
            if (usuario is null)
            {
                return false;
            }

            usuario.EmailConfirmado = true;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> AtualizarPreferenciaNotificacaoTarefaPorEmailAsync(string codigo, bool receberNotificacao)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(usr => usr.Codigo == codigo && !usr.Inativo);
            if (usuario is null)
            {
                return false;
            }

            usuario.ReceberNotificacaoTarefaPorEmail = receberNotificacao;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> AtualizarPreferenciasNotificacaoTarefaAsync(string codigo, bool receberNotificacaoInterna, bool receberNotificacaoPorEmail)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(usr => usr.Codigo == codigo && !usr.Inativo);
            if (usuario is null)
            {
                return false;
            }

            usuario.ReceberNotificacaoInternaTarefa = receberNotificacaoInterna;
            usuario.ReceberNotificacaoTarefaPorEmail = receberNotificacaoPorEmail;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoverUsuarioAsync(Usuario usuario)
        {
            _context.Usuarios.Remove(usuario);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Usuario> ObterUsuarioPorIdAsync(int? id)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(usr => usr.Id == id && !usr.Inativo);
        }

        public async Task<Usuario> ObterUsuarioPorCodigoAsync(string codigo)
        {
            return await _context.Usuarios
                .Include(rel => rel.UsuarioCargoDepartamentos)
                    .ThenInclude(crg => crg.Cargo)
                        .ThenInclude(dpt => dpt.Departamento)
                .Include(rel => rel.UsuarioCargoDepartamentos)
                    .ThenInclude(rel => rel.Departamento)
                .Include(usr => usr.GestorImediato)
                .AsNoTracking()
                .FirstOrDefaultAsync(usr => usr.Codigo == codigo && !usr.Inativo);
        }

        public async Task<Usuario> ObterUsuarioGeralPorCodigoAsync(string codigo)
        {
            return await _context.Usuarios
                .Include(rel => rel.UsuarioCargoDepartamentos)
                    .ThenInclude(crg => crg.Cargo)
                        .ThenInclude(dpt => dpt.Departamento)
                .Include(rel => rel.UsuarioCargoDepartamentos)
                    .ThenInclude(rel => rel.Departamento)
                .Include(usr => usr.GestorImediato)
                .AsNoTracking()
                .FirstOrDefaultAsync(usr => usr.Codigo == codigo);
        }

        public async Task<Usuario> ObterInativoPorEmailAsync(string email)
        {
            return await _context.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(usr => usr.Email == email && usr.Inativo);
        }

        public async Task<Usuario> ObterUsuarioGeralPorEmailAsync(string email)
        {
            var emailNormalizado = email.ToUpperInvariant();
            return await _context.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(usr => usr.Email.ToUpper() == emailNormalizado);
        }

        public async Task<IEnumerable<Usuario>> ObterUsuariosAsync()
        {
            return await _context.Usuarios
                .Include(rel => rel.UsuarioCargoDepartamentos)
                    .ThenInclude(crg => crg.Cargo)
                        .ThenInclude(dpt => dpt.Departamento)
                .Include(rel => rel.UsuarioCargoDepartamentos)
                    .ThenInclude(rel => rel.Departamento)
                .Include(usr => usr.GestorImediato)
                .Where(usr => !usr.Inativo)
                .ToListAsync();
        }

        public async Task<bool> VerificarEmailExistenteAsync(string email)
        {
            var emailNormalizado = email.ToUpperInvariant();
            return await _context.Usuarios.AnyAsync(
                usuario => usuario.Email.ToUpper() == emailNormalizado);
        }

        public async Task<List<UsuarioIdentity>> ObterTodosUsuariosDoIdentity()
        {
            return await (from au in _context.Users
                          join u in _context.Usuarios
                              .Include(r => r.UsuarioCargoDepartamentos)
                                  .ThenInclude(crg => crg.Cargo)
                                      .ThenInclude(dpt => dpt.Departamento)
                              .Include(r => r.UsuarioCargoDepartamentos)
                                  .ThenInclude(rel => rel.Departamento)
                          on au.UserName equals u.Codigo
                          where !u.Inativo
                          select new UsuarioIdentity
                          {
                              Codigo = u.Codigo,
                              Nome = u.Nome,
                              Sobrenome = u.Sobrenome,
                              Email = u.Email,
                              DataNascimento = u.DataNascimento,
                              UsuarioCargoDepartamentos = u.UsuarioCargoDepartamentos
                          })
                          .OrderByDescending(c => c.Codigo)
                          .ToListAsync();
        }
    }
}
