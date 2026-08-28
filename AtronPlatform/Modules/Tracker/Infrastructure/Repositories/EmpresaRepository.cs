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

        public Task<SolicitacaoEmpresa?> ObterUltimaSolicitacaoAsync(int usuarioId, string usuarioCodigo)
            => context.SolicitacoesEmpresa.AsNoTracking()
                .Include(solicitacao => solicitacao.Empresa)
                .Where(solicitacao => solicitacao.UsuarioId == usuarioId
                    && solicitacao.UsuarioCodigo == usuarioCodigo)
                .OrderByDescending(solicitacao => solicitacao.CriadaEm)
                .FirstOrDefaultAsync();

        public async Task CriarSolicitacaoAsync(SolicitacaoEmpresa solicitacao)
        {
            context.Entry(solicitacao.Empresa).State = EntityState.Unchanged;
            context.Entry(solicitacao.Usuario).State = EntityState.Unchanged;
            context.SolicitacoesEmpresa.Add(solicitacao);
            await context.SaveChangesAsync();
        }

        public Task<UsuarioEmpresa?> ObterResponsavelAsync(int empresaId)
            => context.UsuariosEmpresas.AsNoTracking()
                .Include(vinculo => vinculo.Usuario)
                .SingleOrDefaultAsync(vinculo => vinculo.EmpresaId == empresaId
                    && vinculo.Papel == Domain.Enums.PapelUsuarioEmpresa.Responsavel
                    && vinculo.Status == Domain.Enums.StatusUsuarioEmpresa.Ativo);

        public async Task<IReadOnlyList<SolicitacaoEmpresa>> ObterSolicitacoesPendentesAsync(int empresaId)
            => await context.SolicitacoesEmpresa.AsNoTracking()
                .Include(solicitacao => solicitacao.Empresa)
                .Include(solicitacao => solicitacao.Usuario)
                .Where(solicitacao => solicitacao.EmpresaId == empresaId
                    && solicitacao.Status == Domain.Enums.StatusSolicitacaoEmpresa.Pendente)
                .OrderBy(solicitacao => solicitacao.CriadaEm)
                .ToListAsync();

        public Task<SolicitacaoEmpresa?> ObterSolicitacaoPendenteAsync(int solicitacaoId, int empresaId)
            => context.SolicitacoesEmpresa
                .Include(solicitacao => solicitacao.Empresa)
                .Include(solicitacao => solicitacao.Usuario)
                .SingleOrDefaultAsync(solicitacao => solicitacao.Id == solicitacaoId
                    && solicitacao.EmpresaId == empresaId
                    && solicitacao.Status == Domain.Enums.StatusSolicitacaoEmpresa.Pendente);

        public async Task AprovarSolicitacaoAsync(SolicitacaoEmpresa solicitacao, UsuarioEmpresa vinculo)
        {
            context.UsuariosEmpresas.Add(vinculo);
            context.Entry(solicitacao).Property(item => item.Status).CurrentValue = solicitacao.Status;
            await context.SaveChangesAsync();
        }

        public async Task AtualizarSolicitacaoAsync(SolicitacaoEmpresa solicitacao)
        {
            context.SolicitacoesEmpresa.Update(solicitacao);
            await context.SaveChangesAsync();
        }
    }
}
