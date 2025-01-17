﻿using Atron.Application.DTO;

namespace ExternalServices.Interfaces
{
    /// <summary>
    /// Interface dos processos e fluxos do módulo de Departamentos
    /// </summary>
    public interface IDepartamentoExternalService
    {
        /// <summary>
        /// Método que obtém todos os departamentos
        /// </summary>
        /// <returns>Retorna uma lista de departamentos</returns>
        Task<List<DepartamentoDTO>> ObterTodos();

        Task<DepartamentoDTO> ObterPorCodigo(string codigo);

        /// <summary>
        /// Método que cria um departamento
        /// </summary>
        /// <param name="departamento">DTO que será enviado para criação</param>
        Task Criar(DepartamentoDTO departamento);

        /// <summary>
        /// Método que atualiza um departamento existente
        /// </summary>
        /// <param name="codigo">Código do departamento</param>
        /// <param name="departamentoDTO">DTO que será enviado para atualização</param>
        Task Atualizar(string codigo, DepartamentoDTO departamentoDTO);

        /// <summary>
        /// Método que remove um departamento por código
        /// </summary>
        /// <param name="codigo">Código do departamento informado</param>  
        Task Remover(string codigo);
    }
}