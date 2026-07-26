using AtronTracker.Infrastructure.Context;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class SolicitacaoObtencaoTarefaRepository(AtronDbContext context) : Repository<SolicitacaoObtencaoTarefa>(context),
        ISolicitacaoObtencaoTarefaRepository
    {
        private readonly AtronDbContext _context = context;

        public async Task<bool> ExisteSolicitacaoPendenteParaTarefaAsync(int tarefaId)
        {
            return await _context.Set<SolicitacaoObtencaoTarefa>().AnyAsync(sol =>
                        sol.TarefaId == tarefaId &&
                        sol.Status == (int)StatusSolicitacaoObtencaoTarefa.Pendente);
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
                    sol.Status == (int)StatusSolicitacaoObtencaoTarefa.Pendente)
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
                    sol.Status == (int)StatusSolicitacaoObtencaoTarefa.Pendente);

            if (solicitacao is null || solicitacao.Tarefa.UsuarioId.HasValue)
            {
                return false;
            }

            solicitacao.Status = (int)StatusSolicitacaoObtencaoTarefa.Aprovada;
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
                    sol.Status == (int)StatusSolicitacaoObtencaoTarefa.Pendente);

            if (solicitacao is null)
            {
                return false;
            }

            solicitacao.Status = (int)StatusSolicitacaoObtencaoTarefa.Recusada;
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
    }
}
