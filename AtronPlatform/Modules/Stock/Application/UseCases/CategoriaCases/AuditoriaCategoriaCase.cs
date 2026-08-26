using AtronStock.Application.DTO.Request;
using AtronStock.Application.Resources;
using AtronStock.Domain.Entities;
using Shared.Application.DTOS.Common;
using Shared.Application.Interfaces.Service;
using Shared.Extensions;
using System.Transactions;

namespace AtronStock.Application.UseCases.CategoriaCases
{
    public sealed class AuditoriaCategoriaCase(IAuditoriaService auditoriaService)
    {
        private const string CategoriaContexto = nameof(Categoria);

        private readonly IAuditoriaService _auditoriaService = auditoriaService;

        public Task RegistrarCriacaoAsync(Categoria categoria)
            => _auditoriaService.RegistrarServiceAsync(CriarAuditoria(
                categoria.Codigo,
                string.Format(
                    CategoriaResource.HistoricoCriacao,
                    categoria.Codigo,
                    DateTime.Now)));

        public Task RegistrarAtualizacaoAsync(
            Categoria categoria,
            CategoriaRequest request)
            => _auditoriaService.AtualizarServiceAsync(CriarAuditoria(
                categoria.Codigo,
                string.Format(
                    CategoriaResource.HistoricoAtualizacao,
                    categoria.Codigo,
                    DateTime.Now,
                    request.Descricao,
                    request.Status.GetDescription())));

        public Task RegistrarStatusAlteradoAsync(Categoria categoria)
            => _auditoriaService.AtualizarServiceAsync(CriarAuditoria(
                categoria.Codigo,
                string.Format(
                    CategoriaResource.HistoricoStatusAlterado,
                    categoria.Codigo,
                    categoria.Status.GetDescription(),
                    DateTime.Now)));

        public async Task RegistrarInativacaoRecusadaAsync(Categoria categoria)
        {
            using var transacaoSuprimida = new TransactionScope(
                TransactionScopeOption.Suppress,
                TransactionScopeAsyncFlowOption.Enabled);

            await _auditoriaService.AtualizarServiceAsync(CriarAuditoria(
                categoria.Codigo,
                string.Format(
                    CategoriaResource.HistoricoInativacaoRecusada,
                    categoria.Codigo,
                    DateTime.Now)));

            transacaoSuprimida.Complete();
        }

        public Task RegistrarRemocaoAsync(Categoria categoria)
            => _auditoriaService.RemoverServiceAsync(CriarAuditoria(
                categoria.Codigo,
                string.Format(
                    CategoriaResource.HistoricoRemocao,
                    categoria.Codigo,
                    DateTime.Now)));

        private static AuditoriaDTO CriarAuditoria(
            string codigo,
            string descricao)
            => new()
            {
                CodigoRegistro = codigo,
                Contexto = CategoriaContexto,
                Historico = new HistoricoDTO
                {
                    CodigoRegistro = codigo,
                    Contexto = CategoriaContexto,
                    Descricao = descricao
                }
            };
    }
}
