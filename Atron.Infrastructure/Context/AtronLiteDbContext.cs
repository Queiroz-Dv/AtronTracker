using Atron.Domain.Entities;
using Atron.Infrastructure.Interfaces;
using Atron.Infrastructure.Models;
using LiteDB;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Shared.Models.ApplicationModels;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Context
{
    public class AtronLiteDbContext : LiteDataSetContext, ILiteDbContext, IUserStore<ApplicationUser>,
    IUserPasswordStore<ApplicationUser>, IRoleStore<ApplicationRole>
    {
        public LiteDatabase _db;

        public AtronLiteDbContext(LiteDatabase db)
        {
            _db = db;

            Departamentos = new LiteDbSet<Departamento>(_db, "Departamentos");
            Cargos = new LiteDbSet<Cargo>(_db, "Cargos");
            Usuarios = new LiteDbSet<Usuario>(_db, "Usuarios");
            Users = new LiteDbSet<ApplicationUser>(_db, "AppUsers");
            Roles = new LiteDbSet<ApplicationRole>(_db, "AppRoles");
            UsuarioCargoDepartamentos = new LiteDbSet<UsuarioCargoDepartamento>(_db, "UsuarioCargoDepartamentos");

            EnsureIndexes();
        }

        public void EnsureIndexes()
        {
            EnsureDepartamentoIndex();
            EnsureCargoIndex();
            EnsudereIdentityIndexes();
            EnsureUsuarioIndexes();
        }

        private void EnsureUsuarioIndexes()
        {
            var usuarios = GetCollection<Usuario>("Usuarios");
            usuarios.EnsureIndex(u => u.Codigo, unique: true);
            usuarios.EnsureIndex(u => u.Email, unique: true);
        }

        private void EnsudereIdentityIndexes()
        {
            var identityUsers = GetCollection<ApplicationUser>("AppUsers");
            identityUsers.EnsureIndex(u => u.NormalizedUserName, unique: true);
            identityUsers.EnsureIndex(u => u.NormalizedEmail, unique: true);
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

        public Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken ct)
        {
            Users.InsertAsync(user);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken ct)
        {
            Users.UpdateAsync(user);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken ct)
        {
            Users.DeleteAsync(user.Id);
            return Task.FromResult(IdentityResult.Success);
        }

        public async Task<ApplicationUser> FindByIdAsync(string id, CancellationToken ct)
        {
            return await Users.FindByIdAsync(id);
        }

        public async Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken ct)
        {
            return await Users.FindOneAsync(u => u.NormalizedUserName == normalizedUserName);
        }


        // -- PasswordStore --

        public Task SetPasswordHashAsync(ApplicationUser user, string hash, CancellationToken ct)
        {
            user.PasswordHash = hash;
            var atualizado = Users.UpdateAsync(user).Result;
            return atualizado
                ? Task.CompletedTask
                : Task.FromException(new InvalidOperationException("Erro ao atualizar o usuário com a nova senha."));
        }

        public Task<string> GetPasswordHashAsync(ApplicationUser user, CancellationToken ct)
            => Task.FromResult(user.PasswordHash);

        public Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken ct)
            => Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));

        // -- outros métodos obrigatórios (sem-op ou throw) --

        public void Dispose() { /* nada a liberar aqui */ }

        public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken ct)
            => Task.FromResult(user.Id.ToString());

        public Task<string> GetUserNameAsync(ApplicationUser user, CancellationToken ct)
            => Task.FromResult(user.UserName);

        public Task SetUserNameAsync(ApplicationUser user, string name, CancellationToken ct)
        {
            user.UserName = name;
            return Task.CompletedTask;
        }

        public Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken ct)
            => Task.FromResult(user.NormalizedUserName);

        public Task SetNormalizedUserNameAsync(ApplicationUser user, string normalized, CancellationToken ct)
        {
            user.NormalizedUserName = normalized;
            return Task.CompletedTask;
        }

        public Task<IdentityResult> CreateAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> DeleteAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetRoleIdAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedRoleNameAsync(ApplicationRole role, string normalizedName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetRoleNameAsync(ApplicationRole role, string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(ApplicationRole role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<ApplicationRole> IRoleStore<ApplicationRole>.FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<ApplicationRole> IRoleStore<ApplicationRole>.FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}