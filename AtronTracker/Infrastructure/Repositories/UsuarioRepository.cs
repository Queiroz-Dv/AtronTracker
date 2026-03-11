
using AtronTracker.Infrastructure.Context;
using Domain.Entities;
using Domain.Interfaces.UsuarioInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    /// <summary>
    /// Repository para operações de persistência de Usuário
    /// </summary>
    public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
    {
        private AtronDbContext _context;

        public UsuarioRepository(AtronDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Cria um novo usuário no banco de dados
        /// </summary>
        public async Task<bool> CriarUsuarioAsync(Usuario usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        /// <summary>
        /// Atualiza um usuário existente pelo código
        /// </summary>
        public async Task<bool> AtualizarUsuarioAsync(Usuario usuario)
        {            
            _context.Usuarios.Update(usuario);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        /// <summary>
        /// Atualiza o salário do usuário
        /// </summary>
        public async Task<bool> AtualizarSalario(int usuarioId, int quantidadeTotal)
        {
            var usuario = await _context.Usuarios.FirstAsync(usr => usr.Id == usuarioId);
            usuario.SalarioAtual = quantidadeTotal;
            _context.Update(usuario);

            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        /// <summary>
        /// Remove um usuário
        /// </summary>
        public async Task<bool> RemoverUsuarioAsync(Usuario usuario)
        {
            _context.Usuarios.Remove(usuario);
            var removido = await _context.SaveChangesAsync();
            return removido > 0;
        }

        /// <summary>
        /// Obtém um usuário por ID
        /// </summary>
        public async Task<Usuario> ObterUsuarioPorIdAsync(int? id)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(usr => usr.Id == id);
        }

        /// <summary>
        /// Obtém um usuário pelo código
        /// </summary>
        public async Task<Usuario> ObterUsuarioPorCodigoAsync(string codigo)
        {
            return await _context.Usuarios
                .Include(rel => rel.UsuarioCargoDepartamentos)
                    .ThenInclude(crg => crg.Cargo)
                        .ThenInclude(dpt => dpt.Departamento)
                .AsNoTracking()
                .FirstOrDefaultAsync(usr => usr.Codigo == codigo);
        }

        /// <summary>
        /// Obtém todos os usuários
        /// </summary>
        public async Task<IEnumerable<Usuario>> ObterUsuariosAsync()
        {
            return await _context.Usuarios
                .Include(rel => rel.UsuarioCargoDepartamentos)
                .ThenInclude(crg => crg.Cargo)
                .ThenInclude(dpt => dpt.Departamento)
                .ToListAsync();
        }

        /// <summary>
        /// Verifica se existe um e-mail cadastrado
        /// </summary>
        public async Task<bool> VerificarEmailExistenteAsync(string email)
        {
            return await _context.Usuarios.AnyAsync(u => u.Email == email);
        }

        /// <summary>
        /// Obtém todos os usuários do Identity
        /// </summary>
        public async Task<List<UsuarioIdentity>> ObterTodosUsuariosDoIdentity()
        {
            var usuariosIdentity = await (from au in _context.Users
                                          join u in _context.Usuarios.Include(r => r.UsuarioCargoDepartamentos)
                                                .ThenInclude(crg => crg.Cargo)
                                                .ThenInclude(dpt => dpt.Departamento)
                                          on au.UserName equals u.Codigo
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
            return usuariosIdentity;
        }      
    }
}
