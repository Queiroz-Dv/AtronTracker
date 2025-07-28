using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Atron.Infrastructure.Context;
using Atron.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.Interfaces.Accessor;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories
{
    public class TarefaRepository : ITarefaRepository
    {
        private ILiteDbContext context;
        private ILiteUnitOfWork unitOfWork;
        private IServiceAccessor serviceAccessor;

        public TarefaRepository(ILiteDbContext context,
                                ILiteUnitOfWork unitOfWork,
                                IServiceAccessor serviceAccessor)
        {
            this.context = context;
            this.unitOfWork = unitOfWork;
            this.serviceAccessor = serviceAccessor;
        }

        public async Task<bool> AtualizarTarefaAsync(int id, Tarefa tarefa)
        {
            var tarefaBD = await ObterTarefaPorId(id);

            unitOfWork.BeginTransaction();
            tarefaBD.UsuarioId = tarefa.UsuarioId;
            tarefaBD.UsuarioCodigo = tarefa.UsuarioCodigo;
            tarefaBD.Titulo = tarefa.Titulo;
            tarefaBD.Conteudo = tarefa.Conteudo;
            tarefaBD.DataInicial = tarefa.DataInicial;
            tarefaBD.DataFinal = tarefa.DataFinal;
            tarefaBD.TarefaEstadoId = tarefa.TarefaEstadoId;

            try
            {
                var atualizado = await context.Tarefas.UpdateAsync(tarefaBD);
                unitOfWork.Commit();
                return atualizado;
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                serviceAccessor.ObterService<MessageModel>().AdicionarErro(ex.Message);
                return false;
            }          
        }
        
        public async Task<Tarefa> CriarTarefaAsync(Tarefa tarefa)
        {
            //try
            //{
            //    await _context.Tarefas.AddAsync(tarefa);
            //    await _context.SaveChangesAsync();
            //}
            //catch (Exception ex)
            //{

            //    throw ex;
            //}

            return tarefa;
        }

        public async Task<Tarefa> ObterTarefaPorId(int id)
        {
            //return await _context.Tarefas
            //                     .Include(usr => usr.Usuario)
            //                     .ThenInclude(rel => rel.UsuarioCargoDepartamentos)
            //                     .ThenInclude(crg => crg.Cargo)
            //                     .ThenInclude(dpt => dpt.Departamento)
            //                     .FirstOrDefaultAsync(trf => trf.Id == id);
            throw new NotImplementedException();
        }

        public async Task<List<Tarefa>> ObterTodasTarefas()
        {
            //var entidades = await _context.Tarefas
            //    .Include(usr => usr.Usuario) // Obtém o usuário associado a tarefa
            //    .ThenInclude(rel => rel.UsuarioCargoDepartamentos) // Obtém os relacionamentos associados ao usuário
            //    .ThenInclude(crg => crg.Cargo) // Obtém o cargo associado relacionamento do usuário
            //    .ThenInclude(dpt => dpt.Departamento) // Obtém o departamento associado ao relacionamento de cargo
            //    .ToListAsync();
            //return entidades;
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Tarefa>> ObterTodasTarefasPorUsuario(int usuarioId, string usuarioCodigo)
        {
            //return await _context.Tarefas.Where(trf => trf.UsuarioId == usuarioId && trf.UsuarioCodigo == usuarioCodigo).ToListAsync();
            throw new NotImplementedException();
        }
    }
}