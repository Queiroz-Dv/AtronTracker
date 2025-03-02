﻿using Atron.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Domain.Interfaces
{
    public interface ITarefaRepository : IRepository<Tarefa>
    {
        Task<Tarefa> ObterTarefaPorId(int id);

        Task<List<Tarefa>> ObterTodasTarefas();

        Task<IEnumerable<Tarefa>> ObterTodasTarefasPorUsuario(int id, string codigo);

        Task<Tarefa> CriarTarefaAsync(Tarefa tarefa);

        Task<Tarefa> AtualizarTarefaAsync(int id, Tarefa tarefa);
    }
}