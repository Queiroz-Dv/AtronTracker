using AtronStock.Application.Resources;
using AtronStock.Domain.Entities;
using Shared.Application.DTOS.Common;
using Shared.Application.Interfaces.Service;

namespace AtronStock.Application.UseCases.ProdutoCases
{
    public sealed class AuditoriaProdutoCase(IAuditoriaService auditoriaService)
    {
        private const string ProdutoContexto = nameof(Produto);
        private readonly IAuditoriaService _auditoriaService = auditoriaService;

        public Task RegistrarCriacaoAsync(Produto produto)
            => _auditoriaService.RegistrarServiceAsync(CriarAuditoria(
                produto.Codigo,
                string.Format(
                    ProdutoResource.MensagemProdutoCriado,
                    produto.Codigo,
                    DateTime.Now)));

        public Task RegistrarAtualizacaoAsync(Produto produto)
            => _auditoriaService.AtualizarServiceAsync(CriarAuditoria(
                produto.Codigo,
                string.Format(
                    ProdutoResource.HistoricoProdutoAtualizado,
                    produto.Codigo,
                    DateTime.Now)));

        private static AuditoriaDTO CriarAuditoria(string codigo, string descricao)
            => new()
            {
                CodigoRegistro = codigo,
                Contexto = ProdutoContexto,
                Historico = new HistoricoDTO
                {
                    CodigoRegistro = codigo,
                    Contexto = ProdutoContexto,
                    Descricao = descricao
                }
            };
    }
}
