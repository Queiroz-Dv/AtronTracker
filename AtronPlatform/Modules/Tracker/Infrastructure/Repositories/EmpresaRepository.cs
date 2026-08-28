#nullable enable

using AtronTracker.Infrastructure.Context;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public sealed class EmpresaRepository(AtronDbContext context) : IEmpresaRepository
    {
        public Task<Usuario?> ObterUsuarioAsync(string codigo)
            => context.Usuarios.SingleOrDefaultAsync(usuario => usuario.Codigo == codigo);

        public Task<bool> CodigoExisteAsync(string codigo)
            => context.Empresas.AnyAsync(empresa => empresa.Codigo == codigo);

        public Task<UsuarioEmpresa?> ObterVinculoAsync(int usuarioId, string usuarioCodigo)
            => context.UsuariosEmpresas.AsNoTracking().Include(vinculo => vinculo.Empresa)
                .SingleOrDefaultAsync(vinculo => vinculo.UsuarioId == usuarioId
                    && vinculo.UsuarioCodigo == usuarioCodigo);

        public async Task CriarAsync(Empresa empresa)
        {
            foreach (var vinculo in empresa.Usuarios)
            {
                var usuarioEntry = context.Entry(vinculo.Usuario);
                if (usuarioEntry.State == EntityState.Detached)
                    usuarioEntry.State = EntityState.Unchanged;
            }

            context.Empresas.Add(empresa);
            await context.SaveChangesAsync();
        }
    }
}

