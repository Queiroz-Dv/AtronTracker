#nullable enable

using AtronStock.Domain.Entities;
using AtronStock.Domain.Interfaces;
using Shared.Application.Interfaces.Service;

namespace Stock.Tests.TestSupport.Fakes;

internal sealed class LoteProdutoRepositoryFake : ILoteProdutoRepository
{
    public IReadOnlyCollection<string> CodigosLoteExistentes { get; set; } = [];
    public IReadOnlyCollection<string> CodigosProdutoExistentes { get; set; } = [];
    public LoteProduto? LoteAdicionado { get; private set; }
    public bool AdicaoBemSucedida { get; set; } = true;

    public Task<IReadOnlyCollection<string>> ObterCodigosPorPrefixoAsync(string prefixo)
        => Task.FromResult(CodigosLoteExistentes);

    public Task<IReadOnlyCollection<string>> ObterCodigosProdutosExistentesAsync(
        IReadOnlyCollection<string> codigos)
        => Task.FromResult(CodigosProdutoExistentes);

    public Task<bool> AdicionarAsync(LoteProduto lote)
    {
        LoteAdicionado = lote;
        lote.Id = 42;
        return Task.FromResult(AdicaoBemSucedida);
    }
}

internal sealed class ProcessamentoProdutoLoteRepositoryFake
    : IProcessamentoProdutoLoteRepository
{
    public ProcessamentoProdutoLote? ProcessamentoAdicionado { get; private set; }
    public ProcessamentoProdutoLote? Processamento { get; set; }
    public bool AdicaoBemSucedida { get; set; } = true;
    public ICollection<ProcessamentoProdutoLote> ProcessamentosDoSolicitante { get; set; } = [];
    public string? UltimoSolicitanteConsultado { get; private set; }

    public Task<bool> AdicionarAsync(ProcessamentoProdutoLote processamento)
    {
        ProcessamentoAdicionado = processamento;
        processamento.Id = 7;
        return Task.FromResult(AdicaoBemSucedida);
    }

    public Task<ProcessamentoProdutoLote?> ObterPorIdAsync(int id)
        => Task.FromResult(Processamento?.Id == id ? Processamento : null);

    public Task<ProcessamentoProdutoLote?> ReservarProximoDisponivelAsync(
        DateTimeOffset agora,
        TimeSpan duracaoReserva)
    {
        if (Processamento is not null)
            Processamento.Reservar(agora, duracaoReserva, Guid.NewGuid());
        return Task.FromResult(Processamento);
    }

    public Task<bool> AtualizarAsync(ProcessamentoProdutoLote processamento)
        => Task.FromResult(true);

    public Task<ICollection<ProcessamentoProdutoLote>> ObterMeusAsync(
        string solicitanteCodigo)
    {
        UltimoSolicitanteConsultado = solicitanteCodigo;
        return Task.FromResult(ProcessamentosDoSolicitante);
    }

    public Task<ProcessamentoProdutoLote?> ObterPorIdDoSolicitanteAsync(
        int id,
        string solicitanteCodigo)
    {
        UltimoSolicitanteConsultado = solicitanteCodigo;
        return Task.FromResult(ProcessamentosDoSolicitante.FirstOrDefault(item => item.Id == id));
    }
}

internal sealed class UserAccessorFake(string codigo) : IUserAccessor
{
    public string ObterLogadoUsuario() => codigo;
    public string ObterCodigoUsuarioLogado() => codigo;
}

internal sealed class FixedTimeProvider(DateTimeOffset agora) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => agora;
    public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;
}
