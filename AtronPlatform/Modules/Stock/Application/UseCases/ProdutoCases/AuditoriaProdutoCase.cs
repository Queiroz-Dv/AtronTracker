using AtronStock.Application.Resources;
using AtronStock.Domain.Entities;
using Shared.Application.Messaging;
using Shared.Domain.Events.Auditoria;
using System;
using System.Threading.Tasks;

namespace AtronStock.Application.UseCases.ProdutoCases
{
    public sealed class AuditoriaProdutoCase(IEventBus eventBus)
    {
        private const string ProdutoContexto = nameof(Produto);
        private readonly IEventBus _eventBus = eventBus;

        public Task RegistrarCriacaoAsync(Produto produto)
            => _eventBus.PublicarAsync(new AuditoriaRegistradaEvent(
                produto.Codigo,
                ProdutoContexto,
                string.Format(
                    ProdutoResource.MensagemProdutoCriado,
                    produto.Codigo,
                    DateTime.Now))).AsTask();

        public Task RegistrarAtualizacaoAsync(Produto produto)
            => _eventBus.PublicarAsync(new AuditoriaAtualizadaEvent(
                produto.Codigo,
                ProdutoContexto,
                string.Format(
                    ProdutoResource.HistoricoProdutoAtualizado,
                    produto.Codigo,
                    DateTime.Now))).AsTask();
    }
}
