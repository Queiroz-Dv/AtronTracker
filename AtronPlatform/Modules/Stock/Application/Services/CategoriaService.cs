using AtronStock.Application.DTO.Request;
using AtronStock.Application.Interfaces;
using AtronStock.Application.UseCases.CategoriaCases;
using Shared.Domain.ValueObjects;

namespace AtronStock.Application.Services
{
    public class CategoriaService(
        CriarCategoriaCase criarCategoria,
        AtualizarCategoriaCase atualizarCategoria,
        ExcluirCategoriaCase excluirCategoria,
        AtivarInativarCategoriaCase ativarInativarCategoria,
        ObterCategoriaCase obterCategoria) : ICategoriaService
    {
        private readonly CriarCategoriaCase _criarCategoria = criarCategoria;
        private readonly AtualizarCategoriaCase _atualizarCategoria = atualizarCategoria;
        private readonly ExcluirCategoriaCase _excluirCategoria = excluirCategoria;
        private readonly AtivarInativarCategoriaCase _ativarInativarCategoria = ativarInativarCategoria;
        private readonly ObterCategoriaCase _obterCategoria = obterCategoria;

        public Task<Resultado> CriarAsync(CategoriaRequest request)
            => _criarCategoria.ExecutarAsync(request);

        public Task<Resultado> AtualizarAsync(CategoriaRequest request)
            => _atualizarCategoria.ExecutarAsync(request);

        public Task<Resultado> ExcluirAsync(string codigo)
            => _excluirCategoria.ExecutarAsync(codigo);

        public Task<Resultado> AtivarInativarAsync(string codigo, bool ativar)
            => _ativarInativarCategoria.ExecutarAsync(codigo, ativar);

        public Task<Resultado<ICollection<CategoriaRequest>>> ObterTodasAsync()
            => _obterCategoria.ObterTodasAsync();

        public Task<Resultado<ICollection<CategoriaRequest>>> ObterInativasAsync()
            => _obterCategoria.ObterInativasAsync();

        public Task<Resultado<CategoriaRequest>> ObterPorCodigoAsync(string codigo)
            => _obterCategoria.ObterPorCodigoAsync(codigo);
    }
}
