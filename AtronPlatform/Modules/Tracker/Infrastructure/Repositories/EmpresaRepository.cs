#nullable enable

using AtronTracker.Infrastructure.Context;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

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

        public Task<SolicitacaoEmpresa?> ObterSolicitacaoPendenteAsync(
            int usuarioId, string usuarioCodigo, int empresaId)
            => context.SolicitacoesEmpresa.SingleOrDefaultAsync(solicitacao =>
                solicitacao.UsuarioId == usuarioId
                && solicitacao.UsuarioCodigo == usuarioCodigo
                && solicitacao.EmpresaId == empresaId
                && solicitacao.Status == Domain.Enums.StatusSolicitacaoEmpresa.Pendente);

        public async Task CriarSolicitacaoAsync(SolicitacaoEmpresa solicitacao)
        {
            context.Entry(solicitacao.Empresa).State = EntityState.Unchanged;
            context.Entry(solicitacao.Usuario).State = EntityState.Unchanged;
            context.SolicitacoesEmpresa.Add(solicitacao);
            await context.SaveChangesAsync();
        }
    }
}

