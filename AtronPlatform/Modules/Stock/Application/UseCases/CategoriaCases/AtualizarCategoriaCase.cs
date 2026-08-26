using AtronStock.Application.DTO.Request;
using AtronStock.Application.Mapping;
using AtronStock.Application.Resources;
using AtronStock.Domain.Enums;
using AtronStock.Domain.Interfaces;
using Shared.Application.Interfaces.Mapping;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using Shared.Extensions;

namespace AtronStock.Application.UseCases.CategoriaCases
{
    public sealed class AtualizarCategoriaCase(
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
            if (messages.TemErros())
                return Resultado.Falha(messages);

            var categoria = await _repository.ObterCategoriaPorCodigoAsync(request.Codigo);
            if (categoria == null)
            {
                return Resultado.Falha(
                    string.Format(CategoriaResource.ErroCategoriaNaoEncontrada, request.Codigo));
            }

            if (request.Status == EStatus.Inativo
                && categoria.Status != EStatus.Inativo
                && await _repository.PossuiProdutosVinculadosAsync(categoria.Id))
            {
                await _auditoriaCategoria.RegistrarInativacaoRecusadaAsync(categoria);
                return Resultado.Falha(string.Format(
                    CategoriaResource.ErroCategoriaEmUso,
                    categoria.Codigo));
            }

            categoria.MapToUpdate(request, _mapper);

            var atualizado = await _repository.AtualizarCategoriaAsync(categoria);
            if (!atualizado)
                return Resultado.Falha(CategoriaResource.ErroInesperadoAtualizar);

            await _auditoriaCategoria.RegistrarAtualizacaoAsync(categoria, request);
        
            return Resultado.Sucesso(request);
        }
    }
}
