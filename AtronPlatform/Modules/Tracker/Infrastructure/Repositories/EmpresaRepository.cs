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

   
        public async Task CriarAsync(Empresa empresa)
        {           
            context.Empresas.Add(empresa);
            await context.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<Empresa>> BuscarAtivasAsync(string? termo)
        {
            var consulta = context.Empresas.AsNoTracking()
                .Where(empresa => empresa.Status == Domain.Enums.StatusEmpresa.Ativa);

            if (!string.IsNullOrWhiteSpace(termo))
                consulta = consulta.Where(empresa => empresa.Codigo.Contains(termo)
                    || empresa.NomeFantasia.Contains(termo));

            return await consulta.OrderBy(empresa => empresa.NomeFantasia).Take(50).ToListAsync();
        }

        public Task<Empresa?> ObterAtivaAsync(int id)
            => context.Empresas.AsNoTracking().SingleOrDefaultAsync(empresa =>
                empresa.Id == id && empresa.Status == Domain.Enums.StatusEmpresa.Ativa);      
    }
}
