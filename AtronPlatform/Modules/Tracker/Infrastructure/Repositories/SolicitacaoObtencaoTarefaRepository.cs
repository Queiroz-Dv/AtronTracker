using AtronTracker.Infrastructure.Context;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class SolicitacaoObtencaoTarefaRepository(AtronDbContext context) : ISolicitacaoObtencaoTarefaRepository
    {
        private readonly AtronDbContext _context = context;

        public async Task<bool> ExisteSolicitacaoPendenteParaTarefaAsync(int tarefaId)
        {
            return await _context.Set<SolicitacaoObtencaoTarefa>().AnyAsync(sol =>
                        sol.TarefaId == tarefaId &&
                        sol.Status == StatusSolicitacaoObtencaoTarefa.Pendente);
        }

        public async Task<SolicitacaoObtencaoTarefa> ObterPorIdAsync(int id)
        {
            return await QueryComRelacionamentos().FirstOrDefaultAsync(sol => sol.Id == id);
        }

        public async Task<IEnumerable<SolicitacaoObtencaoTarefa>> ObterPendentesPorAprovadorAsync(int aprovadorId, string aprovadorCodigo)
        {
            return await QueryComRelacionamentos()
                .Where(sol =>
                    sol.AprovadorId == aprovadorId &&
                    sol.AprovadorCodigo == aprovadorCodigo &&
                    sol.Status == StatusSolicitacaoObtencaoTarefa.Pendente)
                .OrderByDescending(sol => sol.DataSolicitacao)
                .ToListAsync();
        }

        public async Task<bool> CriarAsync(SolicitacaoObtencaoTarefa solicitacao)
        {
            await _context.Set<SolicitacaoObtencaoTarefa>().AddAsync(solicitacao);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> AprovarAsync(int id, int usuarioId, string usuarioCodigo)
        {
            var solicitacao = await _context.Set<SolicitacaoObtencaoTarefa>()
                .Include(sol => sol.Tarefa)
                .FirstOrDefaultAsync(sol =>
                    sol.Id == id &&
                    sol.AprovadorId == usuarioId &&
                    sol.AprovadorCodigo == usuarioCodigo &&
                    sol.Status == StatusSolicitacaoObtencaoTarefa.Pendente);

            if (solicitacao is null || solicitacao.Tarefa.UsuarioId.HasValue)
            {
                return false;
            }

            solicitacao.Status = StatusSolicitacaoObtencaoTarefa.Aprovada;
            solicitacao.DataDecisao = DateTime.Now;
            solicitacao.Tarefa.AprovarObtencao(
                solicitacao.SolicitanteId,
                solicitacao.SolicitanteCodigo);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RecusarAsync(int id, int usuarioId, string usuarioCodigo)
        {
            var solicitacao = await _context.Set<SolicitacaoObtencaoTarefa>()
                .FirstOrDefaultAsync(sol =>
                    sol.Id == id &&
                    sol.AprovadorId == usuarioId &&
                    sol.AprovadorCodigo == usuarioCodigo &&
                    sol.Status == StatusSolicitacaoObtencaoTarefa.Pendente);

            if (solicitacao is null)
            {
                return false;
            }

            solicitacao.Status = StatusSolicitacaoObtencaoTarefa.Recusada;
            solicitacao.DataDecisao = DateTime.Now;

            return await _context.SaveChangesAsync() > 0;
        }

        private IQueryable<SolicitacaoObtencaoTarefa> QueryComRelacionamentos()
        {
            return _context.Set<SolicitacaoObtencaoTarefa>()
                .Include(sol => sol.Tarefa)
                    .ThenInclude(trf => trf.EstadoDaTarefa)
                .Include(sol => sol.Tarefa)
                    .ThenInclude(trf => trf.Departamento)
                .Include(sol => sol.Tarefa)
                    .ThenInclude(trf => trf.Cargo)
                .Include(sol => sol.Tarefa)
                    .ThenInclude(trf => trf.Usuario)
                .Include(sol => sol.Solicitante)
                    .ThenInclude(usr => usr.UsuarioCargoDepartamentos)
                    .ThenInclude(rel => rel.Cargo)
                    .ThenInclude(crg => crg.Departamento)
                .Include(sol => sol.Aprovador);
        }

        public async Task<IEnumerable<SolicitacaoObtencaoTarefa>> ObterPendentesPorAprovadorOuDepartamentosAsync(
            int aprovadorId,
            string aprovadorCodigo,
            IEnumerable<string> departamentoCodigos)
        {
            var codigos = departamentoCodigos
                .Where(codigo => !string.IsNullOrWhiteSpace(codigo))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            return await QueryComRelacionamentos()
                .Where(sol =>
                    sol.Status == StatusSolicitacaoObtencaoTarefa.Pendente &&
                    ((sol.AprovadorId == aprovadorId && sol.AprovadorCodigo == aprovadorCodigo) ||
                     (sol.Tarefa.DepartamentoCodigo != null && codigos.Contains(sol.Tarefa.DepartamentoCodigo))))
                .OrderByDescending(sol => sol.DataSolicitacao)
                .ToListAsync();
        }
    }
}
