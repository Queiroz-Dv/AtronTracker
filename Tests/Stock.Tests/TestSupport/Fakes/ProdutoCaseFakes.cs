using AtronStock.Domain.Entities;
using AtronStock.Domain.Interfaces;
using Shared.Application.Messaging;
using Shared.Domain.Events;
using Shared.Domain.Entities;
using Shared.Domain.ValueObjects;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Stock.Tests.TestSupport.Fakes;

internal sealed class ProdutoRepositoryFake : IProdutoRepository
{
    public Produto? ProdutoPorCodigo { get; set; }
    public Produto? ProdutoAdicionado { get; private set; }
    public Produto? ProdutoAtualizado { get; private set; }
    public ICollection<Produto> Todos { get; set; } = new List<Produto>();

    public Task<Produto?> ObterPorIdAsync(int id)
        => Task.FromResult(Todos.FirstOrDefault(produto => produto.Id == id));

    public Task<Produto?> ObterPorCodigoAsync(string codigo)
        => Task.FromResult(ProdutoPorCodigo);

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
    public ICollection<Categoria> CategoriasSelecionadas { get; set; } = new List<Categoria>();

    public Task<ICollection<Categoria>> ObterPorCodigosAsync(IReadOnlyCollection<string> codigos)
        => Task.FromResult(CategoriasSelecionadas);

    public Task<bool> CriarCategoriaAsync(Categoria categoria) => Task.FromResult(true);
    public Task<ICollection<Categoria>> ObterTodasCategoriasAsync() => Task.FromResult<ICollection<Categoria>>(new List<Categoria>());
    public Task<ICollection<Categoria>> ObterTodasCategoriasInativasAsync() => Task.FromResult<ICollection<Categoria>>(new List<Categoria>());
    public Task<Categoria> ObterCategoriaPorCodigoAsync(string codigo) => Task.FromResult<Categoria>(null!);
    public Task<bool> PossuiProdutosVinculadosAsync(int categoriaId) => Task.FromResult(false);
    public Task<bool> AtualizarCategoriaAsync(Categoria categoria) => Task.FromResult(true);
}

internal sealed class AuditoriaServiceProdutoFake : IEventBus
{
    public List<Shared.Domain.Events.Auditoria.AuditoriaRegistradaEvent> Criacoes { get; } = new();
    public List<Shared.Domain.Events.Auditoria.AuditoriaAtualizadaEvent> Atualizacoes { get; } = new();

    public ValueTask PublicarAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent
    {
        if (@event is Shared.Domain.Events.Auditoria.AuditoriaRegistradaEvent c) Criacoes.Add(c);
        if (@event is Shared.Domain.Events.Auditoria.AuditoriaAtualizadaEvent a) Atualizacoes.Add(a);
        return ValueTask.CompletedTask;
    }
}
