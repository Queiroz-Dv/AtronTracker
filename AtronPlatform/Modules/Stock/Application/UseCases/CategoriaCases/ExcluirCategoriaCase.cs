using AtronStock.Application.Resources;
using AtronStock.Domain.Enums;
using AtronStock.Domain.Interfaces;
using Shared.Domain.ValueObjects;

namespace AtronStock.Application.UseCases.CategoriaCases
{
    public sealed class ExcluirCategoriaCase(
        ICategoriaRepository repository,
        AuditoriaCategoriaCase auditoriaCategoria)
    {
        private readonly ICategoriaRepository _repository = repository;
        private readonly AuditoriaCategoriaCase _auditoriaCategoria = auditoriaCategoria;

        public async Task<Resultado> ExecutarAsync(string codigo)
        {
            var categoria = await _repository.ObterCategoriaPorCodigoAsync(codigo);
            if (categoria == null)
                return Resultado.Falha(
                    string.Format(CategoriaResource.ErroCategoriaNaoEncontrada, codigo));

            categoria.Status = EStatus.Removido;

            var removido = await _repository.AtualizarCategoriaAsync(categoria);
            if (!removido)
                return Resultado.Falha(CategoriaResource.ErroInesperadoRemover);

            await _auditoriaCategoria.RegistrarRemocaoAsync(categoria);

            var mensagens = new NotificationBag();
            mensagens.MensagemRegistroRemovido(codigo);
            return Resultado.Sucesso([.. mensagens.Messages]);
        }
    }
}
