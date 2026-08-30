using AtronStock.Domain.Entities;
using AtronStock.Domain.Interfaces;
using Shared.Application.DTOS.Common;
using Shared.Application.Interfaces.Service;
using Shared.Domain.Entities;
using Shared.Domain.ValueObjects;

namespace Stock.Tests.TestSupport.Fakes;

internal sealed class ProdutoRepositoryFake : IProdutoRepository
{
    public Produto? ProdutoPorCodigo { get; set; }
    public Produto? ProdutoAdicionado { get; private set; }
    public Produto? ProdutoAtualizado { get; private set; }
    public string? UltimoCodigoConsultado { get; private set; }
    public ICollection<Produto> Todos { get; set; } = [];

    public Task<Produto?> ObterPorIdAsync(int id)
        => Task.FromResult(Todos.FirstOrDefault(produto => produto.Id == id));

    public Task<Produto?> ObterPorCodigoAsync(string codigo)
    {
        UltimoCodigoConsultado = codigo;
        return Task.FromResult(ProdutoPorCodigo);
    }

    public Task<ICollection<Produto>> ObterTodosAsync()
        => Task.FromResult(Todos);

    public Task<bool> AdicionarAsync(Produto produto)
    {
        ProdutoAdicionado = produto;
        return Task.FromResult(true);
    }

    public Task<bool> AtualizarAsync(Produto produto)
    {
        ProdutoAtualizado = produto;
        return Task.FromResult(true);
    }
}

internal sealed class CategoriaRepositoryProdutoFake : ICategoriaRepository
{
    public ICollection<Categoria> CategoriasSelecionadas { get; set; } = [];

    public Task<ICollection<Categoria>> ObterPorCodigosAsync(IReadOnlyCollection<string> codigos)
        => Task.FromResult(CategoriasSelecionadas);

    public Task<bool> CriarCategoriaAsync(Categoria categoria) => Task.FromResult(true);
    public Task<ICollection<Categoria>> ObterTodasCategoriasAsync() => Task.FromResult<ICollection<Categoria>>([]);
    public Task<ICollection<Categoria>> ObterTodasCategoriasInativasAsync() => Task.FromResult<ICollection<Categoria>>([]);
    public Task<Categoria> ObterCategoriaPorCodigoAsync(string codigo) => Task.FromResult<Categoria>(null!);
    public Task<bool> PossuiProdutosVinculadosAsync(int categoriaId) => Task.FromResult(false);
    public Task<bool> AtualizarCategoriaAsync(Categoria categoria) => Task.FromResult(true);
}

internal sealed class AuditoriaServiceProdutoFake : IAuditoriaService
{
    public List<IAuditoriaDTO> Criacoes { get; } = [];
    public List<IAuditoriaDTO> Atualizacoes { get; } = [];

    public Task<Resultado> RegistrarServiceAsync(IAuditoriaDTO auditoria)
    {
        Criacoes.Add(auditoria);
        return Task.FromResult(Resultado.Sucesso());
    }

    public Task<Resultado> AtualizarServiceAsync(IAuditoriaDTO auditoria)
    {
        Atualizacoes.Add(auditoria);
        return Task.FromResult(Resultado.Sucesso());
    }

    public Task<Resultado> RemoverServiceAsync(IAuditoriaDTO auditoria)
        => Task.FromResult(Resultado.Sucesso());

    public Task<Resultado<Auditoria>> ObterPorChaveServiceAsync(IAuditoriaDTO documento)
        => Task.FromResult(Resultado<Auditoria>.Falha("Não implementado no fake."));
}
