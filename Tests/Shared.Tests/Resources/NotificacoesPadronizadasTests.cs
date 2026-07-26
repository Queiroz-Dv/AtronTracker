using Shared.Domain.ValueObjects;
using Xunit;

namespace Shared.Tests.Resources;

public class NotificacoesPadronizadasTests
{
    [Fact]
    public void NotificationBagDeveObterMensagensGenericasDoResource()
    {
        var notificacoes = new NotificationBag();

        notificacoes.MensagemRegistroSalvo("Produto");
        notificacoes.MensagemRegistroAtualizado("Produto");
        notificacoes.MensagemRegistroNaoEncontrado("PRD001");
        notificacoes.MensagemRegistroRemovido("PRD001");
        notificacoes.MensagemRegistroInvalido("PRD001");
        notificacoes.MensagemRegistroNaoExiste("PRD001");

        Assert.Collection(notificacoes.Messages,
            mensagem => Assert.Equal("Produto salvo com sucesso.", mensagem.Descricao),
            mensagem => Assert.Equal("Registro Produto atualizado com sucesso.", mensagem.Descricao),
            mensagem => Assert.Equal("Registro PRD001 não encontrado.", mensagem.Descricao),
            mensagem => Assert.Equal("Registro PRD001 removido com sucesso", mensagem.Descricao),
            mensagem => Assert.Equal("Registro PRD001 inválido", mensagem.Descricao),
            mensagem => Assert.Equal("Registro PRD001 já existe.", mensagem.Descricao));
    }

    [Fact]
    public void ResultadoDeveObterMensagemDeRegistroSalvoDoResource()
    {
        var resultado = Resultado<string>.Sucesso("dados").ComMensagemRegistroSalvo("PRD001");

        Assert.Contains(resultado.Messages, mensagem => mensagem.Descricao == "Registro PRD001 salvo com sucesso.");
    }
}
