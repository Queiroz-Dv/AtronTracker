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

        public async Task<Tarefa> AtualizarTarefaAsync(int id, Tarefa tarefa)
        {
            var tarefaBD = await ObterTarefaPorId(id);
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
            return await _context.Tarefas
                                 .Include(usr => usr.Usuario)
                                 .ThenInclude(rel => rel.UsuarioCargoDepartamentos)
                                 .ThenInclude(crg => crg.Cargo)
                                 .ThenInclude(dpt => dpt.Departamento)
                                 .FirstOrDefaultAsync(trf => trf.Id == id);

        }

        public async Task<List<Tarefa>> ObterTodasTarefas()
        {
            var entidades = await _context.Tarefas
                .Include(usr => usr.Usuario) // Obtém o usuário associado a tarefa
                .ThenInclude(rel => rel.UsuarioCargoDepartamentos) // Obtém os relacionamentos associados ao usuário
                .ThenInclude(crg => crg.Cargo) // Obtém o cargo associado relacionamento do usuário
                .ThenInclude(dpt => dpt.Departamento) // Obtém o departamento associado ao relacionamento de cargo
                .ToListAsync();
            return entidades;
        }       
    }
}