using AtronStock.Application.DTO.Request;
using AtronStock.Application.Resources;
using AtronStock.Domain.Entities;
using Shared.Application.Messaging;
using Shared.Domain.Events.Auditoria;
using Shared.Extensions;
using System.Transactions;

namespace AtronStock.Application.UseCases.CategoriaCases
{
    public sealed class AuditoriaCategoriaCase(IEventBus eventBus)
    {
        private const string CategoriaContexto = nameof(Categoria);
        private readonly IEventBus _eventBus = eventBus;

        public Task RegistrarCriacaoAsync(Categoria categoria)
            => _eventBus.PublicarAsync(new AuditoriaRegistradaEvent(
                categoria.Codigo,
                CategoriaContexto,
                string.Format(
                    CategoriaResource.HistoricoCriacao,
                    categoria.Codigo,
                    DateTime.Now))).AsTask();

        public Task RegistrarAtualizacaoAsync(
            Categoria categoria,
            CategoriaRequest request)
            => _eventBus.PublicarAsync(new AuditoriaAtualizadaEvent(
                categoria.Codigo,
                CategoriaContexto,
                string.Format(
                    CategoriaResource.HistoricoAtualizacao,
                    categoria.Codigo,
                    DateTime.Now,
                    request.Descricao,
                    request.Status.GetDescription()))).AsTask();

        public Task RegistrarStatusAlteradoAsync(Categoria categoria)
            => _eventBus.PublicarAsync(new AuditoriaAtualizadaEvent(
                categoria.Codigo,
                CategoriaContexto,
                string.Format(
                    CategoriaResource.HistoricoStatusAlterado,
                    categoria.Codigo,
                    categoria.Status.GetDescription(),
                    DateTime.Now))).AsTask();

        public async Task RegistrarInativacaoRecusadaAsync(Categoria categoria)
        {
            using var transacaoSuprimida = new TransactionScope(
                TransactionScopeOption.Suppress,
                TransactionScopeAsyncFlowOption.Enabled);

            await _eventBus.PublicarAsync(new AuditoriaAtualizadaEvent(
                categoria.Codigo,
                CategoriaContexto,
                string.Format(
                    CategoriaResource.HistoricoInativacaoRecusada,
                    categoria.Codigo,
                    DateTime.Now)));

            transacaoSuprimida.Complete();
        }

        public Task RegistrarRemocaoAsync(Categoria categoria)
            => _eventBus.PublicarAsync(new AuditoriaRemovidaEvent(
                categoria.Codigo,
                CategoriaContexto,
                string.Format(
                    CategoriaResource.HistoricoRemocao,
                    categoria.Codigo,
                    DateTime.Now))).AsTask();
    }
}
