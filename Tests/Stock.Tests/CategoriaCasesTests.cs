using AtronStock.Application.DTO.Request;
using AtronStock.Application.Mapping;
using AtronStock.Application.UseCases.CategoriaCases;
using AtronStock.Application.Validacoes;
using AtronStock.Domain.Entities;
using AtronStock.Domain.Enums;
using AtronStock.Domain.Interfaces;
using Shared.Application.DTOS.Common;
using Shared.Application.Interfaces.Service;
using Shared.Domain.Entities;
using Shared.Domain.ValueObjects;
using Xunit;

namespace Stock.Tests;

public sealed class CategoriaCasesTests
{
    [Fact]
    public async Task CriarAsync_DeveValidarMapearPersistirEAuditar()
    {
        var repository = new CategoriaRepositoryFake();
        var auditoria = new AuditoriaServiceFake();
        var useCase = new CriarCategoriaCase(
            repository,
            new CategoriaValidador(),
            new CategoriaMapping(),
            new AuditoriaCategoriaCase(auditoria));
        var request = CriarRequest();

        var resultado = await useCase.ExecutarAsync(request);

        Assert.True(resultado.TeveSucesso);
        Assert.Same(request, resultado.Dados);
        Assert.NotNull(repository.CategoriaCriada);
        Assert.Equal(request.Codigo, repository.CategoriaCriada.Codigo);
        Assert.Equal(request.Descricao, repository.CategoriaCriada.Descricao);
        var registro = Assert.Single(auditoria.Registros);
        Assert.Equal(request.Codigo, registro.CodigoRegistro);
        Assert.Equal(nameof(Categoria), registro.Contexto);
    }

    [Fact]
    public async Task AtualizarAsync_DevePreservarIdentidadeAplicarCamposEAuditar()
    {
        var categoria = new Categoria
        {
            Id = 10,
            Codigo = "CAT",
            Descricao = "Antes",
            Status = EStatus.Ativo
        };
        var repository = new CategoriaRepositoryFake
        {
            CategoriaPorCodigo = categoria,
            AtualizacaoBemSucedida = true
        };
        var auditoria = new AuditoriaServiceFake();
        var useCase = new AtualizarCategoriaCase(
            repository,
            new CategoriaValidador(),
            new CategoriaMapping(),
            new AuditoriaCategoriaCase(auditoria));
        var request = CriarRequest(descricao: "Depois", status: EStatus.Inativo);

        var resultado = await useCase.ExecutarAsync(request);

        Assert.True(resultado.TeveSucesso);
        Assert.Equal(10, categoria.Id);
        Assert.Equal("CAT", categoria.Codigo);
        Assert.Equal("Depois", categoria.Descricao);
        Assert.Equal(EStatus.Inativo, categoria.Status);
        Assert.Same(categoria, repository.CategoriaAtualizada);
        Assert.Single(auditoria.Atualizacoes);
    }

    [Fact]
    public async Task AtualizarAsync_DeveRecusarInativacaoQuandoCategoriaPossuiProduto()
    {
        var categoria = new Categoria
        {
            Id = 10,
            Codigo = "CAT",
            Descricao = "Antes",
            Status = EStatus.Ativo
        };
        var repository = new CategoriaRepositoryFake
        {
            CategoriaPorCodigo = categoria,
            PossuiProdutosVinculados = true
        };
        var auditoria = new AuditoriaServiceFake();
        var useCase = new AtualizarCategoriaCase(
            repository,
            new CategoriaValidador(),
            new CategoriaMapping(),
            new AuditoriaCategoriaCase(auditoria));

        var resultado = await useCase.ExecutarAsync(
            CriarRequest(descricao: "Depois", status: EStatus.Inativo));

        Assert.True(resultado.TeveFalha);
        Assert.Equal(EStatus.Ativo, categoria.Status);
        Assert.Equal("Antes", categoria.Descricao);
        Assert.Null(repository.CategoriaAtualizada);
        var registro = Assert.Single(auditoria.Atualizacoes);
        Assert.Contains("recusada", registro.Historico.Descricao);
    }

    [Fact]
    public async Task AtivarInativarAsync_DeveAlterarStatusPersistirEAuditar()
    {
        var categoria = new Categoria
        {
            Codigo = "CAT",
            Descricao = "Categoria",
            Status = EStatus.Inativo
        };
        var repository = new CategoriaRepositoryFake
        {
            CategoriaPorCodigo = categoria,
            AtualizacaoBemSucedida = true
        };
        var auditoria = new AuditoriaServiceFake();
        var useCase = new AtivarInativarCategoriaCase(
            repository,
            new AuditoriaCategoriaCase(auditoria));

        var resultado = await useCase.ExecutarAsync("CAT", true);

        Assert.True(resultado.TeveSucesso);
        Assert.Same(categoria, resultado.Dados);
        Assert.Equal(EStatus.Ativo, categoria.Status);
        Assert.Same(categoria, repository.CategoriaAtualizada);
        Assert.Single(auditoria.Atualizacoes);
    }

    [Fact]
    public async Task AtivarInativarAsync_DeveRecusarInativacaoQuandoCategoriaPossuiProduto()
    {
        var categoria = new Categoria
        {
            Id = 12,
            Codigo = "CAT",
            Descricao = "Categoria",
            Status = EStatus.Ativo
        };
        var repository = new CategoriaRepositoryFake
        {
            CategoriaPorCodigo = categoria,
            PossuiProdutosVinculados = true
        };
        var auditoria = new AuditoriaServiceFake();
        var useCase = new AtivarInativarCategoriaCase(
            repository,
            new AuditoriaCategoriaCase(auditoria));

        var resultado = await useCase.ExecutarAsync("CAT", false);

        Assert.True(resultado.TeveFalha);
        Assert.Equal(EStatus.Ativo, categoria.Status);
        Assert.Null(repository.CategoriaAtualizada);
        var registro = Assert.Single(auditoria.Atualizacoes);
        Assert.Contains("recusada", registro.Historico.Descricao);
    }

    [Fact]
    public async Task AtivarInativarAsync_DeveInativarQuandoCategoriaNaoPossuiProduto()
    {
        var categoria = new Categoria
        {
            Id = 12,
            Codigo = "CAT",
            Descricao = "Categoria",
            Status = EStatus.Ativo
        };
        var repository = new CategoriaRepositoryFake
        {
            CategoriaPorCodigo = categoria,
            PossuiProdutosVinculados = false,
            AtualizacaoBemSucedida = true
        };
        var auditoria = new AuditoriaServiceFake();
        var useCase = new AtivarInativarCategoriaCase(
            repository,
            new AuditoriaCategoriaCase(auditoria));

        var resultado = await useCase.ExecutarAsync("CAT", false);

        Assert.True(resultado.TeveSucesso);
        Assert.Equal(EStatus.Inativo, categoria.Status);
        Assert.Same(categoria, repository.CategoriaAtualizada);
        Assert.Single(auditoria.Atualizacoes);
    }

    [Fact]
    public async Task ExcluirAsync_DeveMarcarComoRemovidaPersistirEAuditar()
    {
        var categoria = new Categoria
        {
            Codigo = "CAT",
            Descricao = "Categoria",
            Status = EStatus.Ativo
        };
        var repository = new CategoriaRepositoryFake
        {
            CategoriaPorCodigo = categoria,
            AtualizacaoBemSucedida = true
        };
        var auditoria = new AuditoriaServiceFake();
        var useCase = new ExcluirCategoriaCase(
            repository,
            new AuditoriaCategoriaCase(auditoria));

        var resultado = await useCase.ExecutarAsync("CAT");

        Assert.True(resultado.TeveSucesso);
        Assert.Equal(EStatus.Removido, categoria.Status);
        Assert.Same(categoria, repository.CategoriaAtualizada);
        Assert.Single(auditoria.Remocoes);
    }

    [Fact]
    public async Task ObterAsync_DeveMapearConsultasParaRequests()
    {
        var ativa = new Categoria
        {
            Codigo = "ATV",
            Descricao = "Ativa",
            Status = EStatus.Ativo
        };
        var inativa = new Categoria
        {
            Codigo = "INA",
            Descricao = "Inativa",
            Status = EStatus.Inativo
        };
        var repository = new CategoriaRepositoryFake
        {
            CategoriaPorCodigo = ativa,
            Todas = [ativa, inativa],
            Inativas = [inativa]
        };
        var useCase = new ObterCategoriaCase(repository, new CategoriaMapping());

        var todas = await useCase.ObterTodasAsync();
        var inativas = await useCase.ObterInativasAsync();
        var porCodigo = await useCase.ObterPorCodigoAsync("ATV");

        Assert.Equal(2, todas.Dados!.Count);
        Assert.Equal("INA", Assert.Single(inativas.Dados!).Codigo);
        Assert.Equal("ATV", porCodigo.Dados!.Codigo);
    }

    private static CategoriaRequest CriarRequest(
        string descricao = "Categoria",
        EStatus status = EStatus.Ativo)
        => new()
        {
            Codigo = "CAT",
            Descricao = descricao,
            Status = status
        };

    private sealed class CategoriaRepositoryFake : ICategoriaRepository
    {
        public Categoria? CategoriaPorCodigo { get; init; }
        public Categoria? CategoriaCriada { get; private set; }
        public Categoria? CategoriaAtualizada { get; private set; }
        public bool AtualizacaoBemSucedida { get; init; }
        public bool PossuiProdutosVinculados { get; init; }
        public ICollection<Categoria> Todas { get; init; } = [];
        public ICollection<Categoria> Inativas { get; init; } = [];

        public Task<bool> CriarCategoriaAsync(Categoria categoria)
        {
            CategoriaCriada = categoria;
            return Task.FromResult(true);
        }

        public Task<ICollection<Categoria>> ObterTodasCategoriasAsync()
            => Task.FromResult(Todas);

        public Task<ICollection<Categoria>> ObterTodasCategoriasInativasAsync()
            => Task.FromResult(Inativas);

        public Task<Categoria> ObterCategoriaPorCodigoAsync(string codigo)
            => Task.FromResult(CategoriaPorCodigo!);

        public Task<ICollection<Categoria>> ObterPorCodigosAsync(
            IReadOnlyCollection<string> codigos)
            => Task.FromResult<ICollection<Categoria>>([]);

        public Task<bool> PossuiProdutosVinculadosAsync(int categoriaId)
            => Task.FromResult(PossuiProdutosVinculados);

        public Task<bool> AtualizarCategoriaAsync(Categoria categoria)
        {
            CategoriaAtualizada = categoria;
            return Task.FromResult(AtualizacaoBemSucedida);
        }
    }

    private sealed class AuditoriaServiceFake : IAuditoriaService
    {
        public List<IAuditoriaDTO> Registros { get; } = [];
        public List<IAuditoriaDTO> Atualizacoes { get; } = [];
        public List<IAuditoriaDTO> Remocoes { get; } = [];

        public Task<Resultado> RegistrarServiceAsync(IAuditoriaDTO auditoria)
        {
            Registros.Add(auditoria);
            return Task.FromResult(Resultado.Sucesso());
        }

        public Task<Resultado> AtualizarServiceAsync(IAuditoriaDTO auditoriaDTO)
        {
            Atualizacoes.Add(auditoriaDTO);
            return Task.FromResult(Resultado.Sucesso());
        }

        public Task<Resultado> RemoverServiceAsync(IAuditoriaDTO auditoriaDTO)
        {
            Remocoes.Add(auditoriaDTO);
            return Task.FromResult(Resultado.Sucesso());
        }

        public Task<Resultado<Auditoria>> ObterPorChaveServiceAsync(IAuditoriaDTO documento)
            => Task.FromResult(Resultado<Auditoria>.Falha("Não implementado no fake."));
    }
}
