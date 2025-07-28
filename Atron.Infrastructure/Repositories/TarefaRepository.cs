using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Atron.Domain.Interfaces.UsuarioInterfaces;
using Atron.Infrastructure.Interfaces;
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
        private readonly IUsuarioRepository usuarioRepository;

        public TarefaRepository(ILiteDbContext context,
                                ILiteUnitOfWork unitOfWork,
                                IServiceAccessor serviceAccessor,
                                IUsuarioRepository usuarioRepository)
        {
            this.context = context;
            this.unitOfWork = unitOfWork;
            this.serviceAccessor = serviceAccessor;
            this.usuarioRepository = usuarioRepository;
        }

        public async Task<bool> AtualizarTarefaAsync(int id, Tarefa tarefa)
        {
            try
            {
                unitOfWork.BeginTransaction();
                var tarefaBD = await context.Tarefas.FindByIdAsync(id);

                tarefaBD.UsuarioId = tarefa.UsuarioId;
                tarefaBD.UsuarioCodigo = tarefa.UsuarioCodigo;
                tarefaBD.Titulo = tarefa.Titulo;
                tarefaBD.Conteudo = tarefa.Conteudo;
                tarefaBD.DataInicial = tarefa.DataInicial;
                tarefaBD.DataFinal = tarefa.DataFinal;
                tarefaBD.TarefaEstadoId = tarefa.TarefaEstadoId;

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

        public async Task<bool> CriarTarefaAsync(Tarefa tarefa)
        {
            try
            {
                unitOfWork.BeginTransaction();
                var gravado = await context.Tarefas.InsertAsync(tarefa);
                unitOfWork.Commit();
                return gravado > 0;
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                serviceAccessor.ObterService<MessageModel>().AdicionarErro(ex.Message);
                return false;
            }           
        }

        public async Task<Tarefa> ObterTarefaPorId(int id)
        {            
            var tarefa = await context.Tarefas.FindByIdAsync(id);
            tarefa.Usuario = await usuarioRepository.ObterUsuarioPorCodigoAsync(tarefa.UsuarioCodigo);
            return tarefa;
        }

        public async Task<List<Tarefa>> ObterTodasTarefas()
        {
            var usuarios = await usuarioRepository.ObterUsuariosAsync();
            var tarefas = await context.Tarefas.FindAllAsync();
            var tarefasComUsuarios = tarefas.Select(tarefa =>
            {
                tarefa.Usuario = usuarios.FirstOrDefault(usr => usr.Codigo == tarefa.UsuarioCodigo);
                return tarefa;
            }).ToList();

            return tarefasComUsuarios;
        }

        public async Task<IEnumerable<Tarefa>> ObterTodasTarefasPorUsuario(int usuarioId, string usuarioCodigo)
        {
            var tarefa = await context.Tarefas.FindAllAsync();
            return tarefa.Where(t => t.UsuarioId == usuarioId && t.UsuarioCodigo == usuarioCodigo).ToList();
        }

        public async Task<bool> ExcluirTarefaAsync(int id)
        {
            try
            {
                unitOfWork.BeginTransaction();
                var deletado = await context.Tarefas.DeleteAsync(id);
                unitOfWork.Commit();
                return deletado;
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                serviceAccessor.ObterService<MessageModel>().AdicionarErro(ex.Message);
                return false;
            }
        }

        public async Task<string> ObterDescricaoTarefaEstado(int tarefaEstadoId)
        {
            var tarefaEstado = await context.TarefasEstados.FindByIdAsync(tarefaEstadoId);
            return tarefaEstado?.Descricao ?? string.Empty;
        }
    }
}