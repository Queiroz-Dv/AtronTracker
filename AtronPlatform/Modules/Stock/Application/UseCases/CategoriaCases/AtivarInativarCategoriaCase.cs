using AtronStock.Domain.Enums;
using AtronStock.Domain.Interfaces;
using Shared.Domain.ValueObjects;
using Shared.Extensions;

namespace AtronStock.Application.UseCases.CategoriaCases
{
    public sealed class AtivarInativarCategoriaCase(
        ICategoriaRepository repository,
        AuditoriaCategoriaCase auditoriaCategoria)
    {
        private readonly ICategoriaRepository _repository = repository;
        private readonly AuditoriaCategoriaCase _auditoriaCategoria = auditoriaCategoria;

        public async Task<Resultado> ExecutarAsync(string codigo, bool ativar)
        {
            var categoria = await _repository.ObterCategoriaPorCodigoAsync(codigo);
            if (categoria == null)
            {                
                return Resultado.Falha().ComMensagemRegistroNaoEncontrado(codigo);
            }

            if (!ativar && await _repository.PossuiProdutosVinculadosAsync(categoria.Id))
            {
                await _auditoriaCategoria.RegistrarInativacaoRecusadaAsync(categoria);
                return Resultado.Falha(string.Format(
                    Resources.CategoriaResource.ErroCategoriaEmUso,
                    categoria.Codigo));
            }

            categoria.Status = ativar ? EStatus.Ativo : EStatus.Inativo;
            await _repository.AtualizarCategoriaAsync(categoria);
            await _auditoriaCategoria.RegistrarStatusAlteradoAsync(categoria);
            
            return Resultado.Sucesso(categoria).ComMensagemRegistroAtualizado(codigo);
        }
    }
}