using Application.DTO;
using Application.Interfaces.Services;
using Application.UseCases.DepartamentoCases;
using Shared.Domain.ValueObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services.EntitiesServices
{
    public class DepartamentoService(
        CriarDepartamentoCase criarDepartamento,
        AtualizarDepartamentoCase atualizarDepartamento,
        ExcluirDepartamentoCase excluirDepartamento,
        ObterDepartamentoCase obterDepartamento) : IDepartamentoService
    {
        private readonly CriarDepartamentoCase _criarDepartamento = criarDepartamento;
        private readonly AtualizarDepartamentoCase _atualizarDepartamento = atualizarDepartamento;
        private readonly ExcluirDepartamentoCase _excluirDepartamento = excluirDepartamento;
        private readonly ObterDepartamentoCase _obterDepartamento = obterDepartamento;

        public Task<Resultado> CriarAsync(DepartamentoDTO departamentoDTO)
            => _criarDepartamento.ExecutarAsync(departamentoDTO);

        public Task<Resultado> AtualizarAsync(string codigo, DepartamentoDTO departamentoDTO)
            => _atualizarDepartamento.ExecutarAsync(codigo, departamentoDTO);

        public Task<Resultado> RemoverAsync(string codigo)
            => _excluirDepartamento.ExecutarAsync(codigo);

        public Task<Resultado<List<DepartamentoDTO>>> ObterTodosAsync()
            => _obterDepartamento.ObterTodosAsync();

        public Task<Resultado<DepartamentoDTO>> ObterPorCodigo(string codigo)
            => _obterDepartamento.ObterPorCodigoAsync(codigo);

        public Task<Resultado<DepartamentoDTO>> ObterPorIdAsync(int? departamentoId)
            => _obterDepartamento.ObterPorIdAsync(departamentoId);

        public Task<Resultado<IEnumerable<DepartamentoDTO>>> ObterDepartamentosPorGestor(string usuarioCodigo)
            => _obterDepartamento.ObterPorGestorAsync(usuarioCodigo);
    }
}