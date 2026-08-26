using AtronStock.Application.DTO.Request;
using AtronStock.Application.Mapping;
using AtronStock.Application.Resources;
using AtronStock.Domain.Interfaces;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;

namespace AtronStock.Application.UseCases.CategoriaCases
{
    public sealed class CriarCategoriaCase(
        ICategoriaRepository repository,
        IValidador<CategoriaRequest> validador,
        CategoriaMapping mapper,
        AuditoriaCategoriaCase auditoriaCategoria)
    {
        private readonly ICategoriaRepository _repository = repository;
        private readonly IValidador<CategoriaRequest> _validador = validador;
        private readonly CategoriaMapping _mapper = mapper;
        private readonly AuditoriaCategoriaCase _auditoriaCategoria = auditoriaCategoria;

        public async Task<Resultado> ExecutarAsync(CategoriaRequest request)
        {
            var messages = _validador.Validar(request);
            if (messages.Any())
                return Resultado.Falha(messages);

            var categoriaExistente = await _repository.ObterCategoriaPorCodigoAsync(request.Codigo);
            if (categoriaExistente != null)
            {
                return Resultado.Falha(
                    string.Format(CategoriaResource.ErroCategoriaJaExiste, request.Codigo));
            }

            var categoria = _mapper.MapToEntity(request);
            await _repository.CriarCategoriaAsync(categoria);
            await _auditoriaCategoria.RegistrarCriacaoAsync(categoria);

            var context = new NotificationBag();
            context.MensagemRegistroSalvo(CategoriaResource.SucessoCadastro);
            return Resultado.Sucesso(request, [.. context.Messages]);
        }
    }
}