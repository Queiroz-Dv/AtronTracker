using AtronTracker.Infrastructure.Context;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class TarefaRepository : Repository<Tarefa>, ITarefaRepository
    {
        private const int EstadoFinalizadaId = 4;
        private readonly AtronDbContext _context;

        public TarefaRepository(AtronDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> AtualizarTarefaAsync(int id, Tarefa tarefa)
        {
            var tarefaBD = await ObterTarefaPorId(id);
            AtualizarEntidadeParaPersistencia(tarefa, tarefaBD);

            try
            {
                _context.Tarefas.Update(tarefaBD);
                var atualizado = await _context.SaveChangesAsync();
                return atualizado > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> CriarTarefaAsync(Tarefa tarefa)
        {
            try
            {
                if (!tarefa.Identificador.HasValue)
                {
                    var ultimoIdentificador = await _context.Tarefas.MaxAsync(trf => trf.Identificador);
                    tarefa.Identificador = (ultimoIdentificador ?? 0) + 1;
                }

                await _context.Tarefas.AddAsync(tarefa);
                var gravado = await _context.SaveChangesAsync();
                return gravado > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Tarefa> ObterTarefaPorId(int id)
        {
            return await _context.Tarefas
                .Include(trf => trf.EstadoDaTarefa)
                .Include(trf => trf.Usuario)
                    .ThenInclude(rel => rel.UsuarioCargoDepartamentos)
                    .ThenInclude(crg => crg.Cargo)
                    .ThenInclude(dpt => dpt.Departamento)
                .Include(trf => trf.Departamento)
                .Include(trf => trf.Cargo)
                .FirstOrDefaultAsync(trf => trf.Id == id);
        }

        public async Task<List<Tarefa>> ObterTodasTarefas()
        {
            return await _context.Tarefas
                .Include(trf => trf.EstadoDaTarefa)
                .Include(trf => trf.Usuario)
                    .ThenInclude(rel => rel.UsuarioCargoDepartamentos)
                    .ThenInclude(crg => crg.Cargo)
                    .ThenInclude(dpt => dpt.Departamento)
                .Include(trf => trf.Departamento)
                .Include(trf => trf.Cargo)
                .ToListAsync();
        }

        public async Task<IEnumerable<Tarefa>> ObterTodasTarefasPorUsuario(int usuarioId, string usuarioCodigo)
        {
            return await _context.Tarefas
                .Include(trf => trf.EstadoDaTarefa)
                .Where(trf => trf.UsuarioId == usuarioId && trf.UsuarioCodigo == usuarioCodigo)
                .ToListAsync();
        }

        public async Task<IEnumerable<Tarefa>> ObterTarefasAtivasPorUsuarioAsync(int usuarioId, string usuarioCodigo)
        {
            return await QueryTarefasComRelacionamentos()
                .Where(trf =>
                    trf.UsuarioId == usuarioId &&
                    trf.UsuarioCodigo == usuarioCodigo &&
                    trf.TarefaEstadoId != EstadoFinalizadaId)
                .OrderByDescending(trf => trf.Identificador)
                .ToListAsync();
        }

        public async Task<IEnumerable<Tarefa>> ObterTarefasAtivasPorSubordinadosDiretosAsync(int gestorId, string gestorCodigo)
        {
            return await QueryTarefasComRelacionamentos()
                .Where(trf =>
                    trf.Usuario != null &&
                    trf.Usuario.GestorImediatoId == gestorId &&
                    trf.Usuario.GestorImediatoCodigo == gestorCodigo &&
                    trf.TarefaEstadoId != EstadoFinalizadaId                    
                    )
                .OrderByDescending(trf => trf.Identificador)
                .ToListAsync();
        }

        public async Task<IEnumerable<Tarefa>> ObterTarefasAtivasDisponiveisParaUsuarioAsync(
            int usuarioId,
            string usuarioCodigo,
            IReadOnlyCollection<int> departamentoIds,
            IReadOnlyCollection<int> cargoIds)
        {
            return await QueryTarefasComRelacionamentos()
                .Where(trf =>
                    trf.UsuarioId == null &&
                    trf.TarefaEstadoId != EstadoFinalizadaId &&
                    trf.DepartamentoId.HasValue &&
                    (
                        (
                            departamentoIds.Contains(trf.DepartamentoId.Value) &&
                            (!trf.CargoId.HasValue || cargoIds.Contains(trf.CargoId.Value))
                        ) ||
                        (
                            trf.Departamento != null &&
                            trf.Departamento.GestorDepartamentoId == usuarioId &&
                            trf.Departamento.GestorDepartamentoCodigo == usuarioCodigo
                        )
                    ))
                .OrderByDescending(trf => trf.Identificador)
                .ToListAsync();
        }

        public async Task<bool> AssumirTarefaAsync(int tarefaId, int usuarioId, string usuarioCodigo)
        {
            var tarefa = await _context.Tarefas
                .FirstOrDefaultAsync(trf =>
                    trf.Id == tarefaId &&
                    trf.UsuarioId == null &&
                    trf.TarefaEstadoId != EstadoFinalizadaId);

            if (tarefa is null)
            {
                return false;
            }

            tarefa.UsuarioId = usuarioId;
            tarefa.UsuarioCodigo = usuarioCodigo;

            return await _context.SaveChangesAsync() > 0;
        }

        private static void AtualizarEntidadeParaPersistencia(Tarefa tarefa, Tarefa tarefaBD)
        {
            tarefaBD.UsuarioId = tarefa.UsuarioId;
            tarefaBD.UsuarioCodigo = tarefa.UsuarioCodigo;
            tarefaBD.Identificador = tarefa.Identificador ?? tarefaBD.Identificador;
            tarefaBD.DestinoInicial = tarefa.DestinoInicial;
            tarefaBD.ExigeAprovacaoParaObter = tarefa.ExigeAprovacaoParaObter;
            tarefaBD.DepartamentoId = tarefa.DepartamentoId;
            tarefaBD.DepartamentoCodigo = tarefa.DepartamentoCodigo;
            tarefaBD.CargoId = tarefa.CargoId;
            tarefaBD.CargoCodigo = tarefa.CargoCodigo;
            tarefaBD.Titulo = tarefa.Titulo;
            tarefaBD.Conteudo = tarefa.Conteudo;
            tarefaBD.DataInicial = tarefa.DataInicial;
            tarefaBD.DataFinal = tarefa.DataFinal;
            tarefaBD.TarefaEstadoId = tarefa.TarefaEstadoId;
        }

        private IQueryable<Tarefa> QueryTarefasComRelacionamentos()
        {
            return _context.Tarefas
                .Include(trf => trf.EstadoDaTarefa)
                .Include(trf => trf.Usuario)
                    .ThenInclude(rel => rel.UsuarioCargoDepartamentos)
                    .ThenInclude(crg => crg.Cargo)
                    .ThenInclude(dpt => dpt.Departamento)
                .Include(trf => trf.Departamento)
                .Include(trf => trf.Cargo);
        }
    }
}
