using Atron.Domain.Entities;
using Atron.Domain.Interfaces.UsuarioInterfaces;
using Atron.Infrastructure.Context;
using Atron.Infrastructure.Interfaces;
using Shared.Interfaces.Accessor;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories
{
    public abstract class UsuarioBaseRepository : Repository<Usuario>
    {
        protected IDataSet<UsuarioIdentity> Users => _facade.LiteDbContext.UsuarioIdentity;
        protected IDataSet<Usuario> Usuarios => _facade.LiteDbContext.Usuarios;
        protected IDataSet<UsuarioCargoDepartamento> UsuarioCargoDepartamentos => _facade.LiteDbContext.UsuarioCargoDepartamentos;

        public UsuarioBaseRepository(ILiteFacade liteFacade, IServiceAccessor serviceAccessor) : base(liteFacade, serviceAccessor)
        { }
    }

    public class UsuarioRepository : UsuarioBaseRepository, IUsuarioRepository
    {
        public UsuarioRepository(ILiteFacade liteFacade,
            IServiceAccessor serviceAccessor) : base(liteFacade, serviceAccessor)
        { }

        public async Task<bool> AtualizarSalario(int usuarioId, int quantidadeTotal)
        {
            var usuario = await Usuarios.FindByIdAsync(usuarioId);
            if (usuario != null)
                usuario.SalarioAtual = quantidadeTotal;

            return await Usuarios.UpdateAsync(usuario);
        }

        public async Task<bool> AtualizarUsuarioAsync(string codigo, Usuario usuario)
        {
            var usuarioBd = await Usuarios.FindOneAsync(usr => usr.Codigo == codigo);
            usuarioBd.Nome = usuario.Nome;
            usuarioBd.Sobrenome = usuario.Sobrenome;
            usuarioBd.DataNascimento = usuario.DataNascimento;
            usuarioBd.SalarioAtual = usuario.SalarioAtual;
            usuarioBd.Email = usuario.Email;
            usuarioBd.UsuarioCargoDepartamentos = usuario.UsuarioCargoDepartamentos;

            return await Usuarios.UpdateAsync(usuarioBd);
        }

        public async Task<bool> CriarUsuarioAsync(Usuario usuario)
        {
                var gravado = await Usuarios.InsertAsync(usuario);
            return gravado > 0;
        }

        public async Task<UsuarioIdentity> ObterUsuarioPorCodigoAsync(string codigo)
        {
            var relacionamentos = await UsuarioCargoDepartamentos.FindAllAsync();
            var applicationUser = await Users.FindOneAsync(usr => usr.Codigo == codigo);
            var usuario = await Usuarios.FindOneAsync(usr => usr.Codigo == codigo);

            foreach (var item in relacionamentos)
            {
                if (item.Usuario.Codigo == usuario.Codigo)
                {
                    usuario.UsuarioCargoDepartamentos.Add(item);
                }
            }

            var usuarioIdentity = new UsuarioIdentity
            {
                Codigo = usuario.Codigo,
                Nome = usuario.Nome,
                Sobrenome = usuario.Sobrenome,
                Email = usuario.Email,
                SalarioAtual = usuario.SalarioAtual,
                DataNascimento = usuario.DataNascimento,
                UsuarioCargoDepartamentos = usuario.UsuarioCargoDepartamentos,
                RefreshToken = applicationUser.RefreshToken,
                RefreshTokenExpireTime = applicationUser.RefreshTokenExpireTime
            };

            return usuarioIdentity;
        }

        public async Task<Usuario> ObterUsuarioPorIdAsync(int? id)
        {
            return await Usuarios.FindByIdAsync(id);
        }

        public async Task<IEnumerable<Usuario>> ObterUsuariosAsync()
        {
            var relacionamentos = await UsuarioCargoDepartamentos.FindAllAsync();
            var usuarios = await Usuarios.FindAllAsync();

            // Monta lista de usuários com seus relacionamentos
            var usuariosComRelacionamentos = usuarios.Select(usr =>
            {
                usr.UsuarioCargoDepartamentos = relacionamentos
                    .Where(rel => rel.UsuarioCodigo == usr.Codigo)
                    .ToList();
                return usr;
            }).ToList();

            return usuariosComRelacionamentos;
        }

        public async Task<bool> RemoverUsuarioAsync(Usuario usuario)
        {
            var usuarioBd = await Usuarios.FindOneAsync(usr => usr.Codigo == usuario.Codigo);
            return await Usuarios.DeleteAsync(usuarioBd.Id);
        }

        public bool UsuarioExiste(string codigo)
        {
            return Usuarios.AnyAsync(usr => usr.Codigo == codigo).Result;
        }

        public async Task<List<UsuarioIdentity>> ObterTodosUsuariosDoIdentity()
        {
            var applicationUsers = (await Users.FindAllAsync()).ToList();
            var relacionamentos = (await UsuarioCargoDepartamentos.FindAllAsync()).ToList();
            var usuarios = (await Usuarios.FindAllAsync()).ToList();

            var usuariosIdentity = new List<UsuarioIdentity>();

            foreach (var user in applicationUsers)
            {
                var usuario = usuarios.FirstOrDefault(cdg => cdg.Codigo == user.Codigo);

                if (usuario is not null)
                {
                    // Preenche os relacionamentos do usuário
                    usuario.UsuarioCargoDepartamentos = relacionamentos
                        .Where(rel => rel.UsuarioCodigo == usuario.Codigo)
                        .ToList();

                    var usuarioIdentity = new UsuarioIdentity
                    {
                        Codigo = usuario.Codigo,
                        Nome = usuario.Nome,
                        Sobrenome = usuario.Sobrenome,
                        Email = usuario.Email,
                        Salario = usuario.Salario,
                        SalarioAtual = usuario.SalarioAtual,
                        DataNascimento = usuario.DataNascimento,
                        UsuarioCargoDepartamentos = usuario.UsuarioCargoDepartamentos,
                        RefreshToken = user.RefreshToken,
                        RefreshTokenExpireTime = user.RefreshTokenExpireTime
                    };

                    usuariosIdentity.Add(usuarioIdentity);
                }
            }

            return usuariosIdentity;
        }
    }
}