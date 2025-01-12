using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Atron.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories
{
    public class TarefaRepository : Repository<Tarefa>, ITarefaRepository
    {
        private readonly AtronDbContext _context;

        public TarefaRepository(AtronDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Tarefa> AtualizarTarefaAsync(Tarefa tarefa)
        {
            var tarefaBD = await ObterTarefaPorId(tarefa.Id);
            AtualizarEntidadeParaPersistencia(tarefa, tarefaBD);
            try
            {
                _context.Tarefas.Update(tarefa);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return new Tarefa();
        }

        private static void AtualizarEntidadeParaPersistencia(Tarefa tarefa, Tarefa tarefaBD)
        {
            tarefaBD.UsuarioId = tarefa.UsuarioId;
            tarefaBD.UsuarioCodigo = tarefa.UsuarioCodigo;
            tarefaBD.Titulo = tarefa.Titulo;
            tarefaBD.Conteudo = tarefa.Conteudo;
            tarefaBD.DataInicial = tarefa.DataInicial;
            tarefaBD.DataFinal = tarefa.DataFinal;
            tarefaBD.TarefaEstadoId = tarefa.TarefaEstadoId;
        }

        public async Task<Tarefa> CriarTarefaAsync(Tarefa tarefa)
        {
            try
            {
                await _context.Tarefas.AddAsync(tarefa);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return tarefa;
        }

        public async Task<Tarefa> ObterTarefaPorId(int id)
        {
            var entidade = await _context.Tarefas            
            .Include(usr => usr.Usuario)
         //   .ThenInclude(crg => crg.Cargo)
         //   .ThenInclude(dpt => dpt.Departamento)
         .FirstOrDefaultAsync(trf => trf.Id == id);
            return entidade;
        }

        public async Task<List<Tarefa>> ObterTodasTarefas()
        {
            var entidades = await _context.Tarefas                
                //.Include(usr => usr.Usuario)
                //.ThenInclude(crg => crg.Cargo)
                //.ThenInclude(dpt => dpt.Departamento)
                .ToListAsync();
            return entidades;
        }
    }
}