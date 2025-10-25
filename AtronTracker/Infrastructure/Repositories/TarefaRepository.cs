using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class TarefaRepository : Repository<Tarefa>, ITarefaRepository
    {
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

        public async Task<bool> CriarTarefaAsync(Tarefa tarefa)
        {
            try
            {
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

        public async Task<IEnumerable<Tarefa>> ObterTodasTarefasPorUsuario(int usuarioId, string usuarioCodigo)
        {
            return await _context.Tarefas.Where(trf => trf.UsuarioId == usuarioId && trf.UsuarioCodigo == usuarioCodigo).ToListAsync();

        }
    }
}