using AtronStock.Application.DTO.Request;
using AtronStock.Application.Mapping;
using AtronStock.Domain.Interfaces;
using Shared.Domain.ValueObjects;

namespace AtronStock.Application.UseCases.CategoriaCases
{
    public sealed class ObterCategoriaCase(
        ICategoriaRepository repository,
        CategoriaMapping mapper)
    {
        private readonly ICategoriaRepository _repository = repository;
        private readonly CategoriaMapping _mapper = mapper;

        public async Task<Resultado<ICollection<CategoriaRequest>>> ObterTodasAsync()
        {
            var categorias = await _repository.ObterTodasCategoriasAsync();
            var requests = _mapper.MapToDtos(categorias).ToList();

            return Resultado<ICollection<CategoriaRequest>>.Sucesso(requests);
        }

        public async Task<Resultado<ICollection<CategoriaRequest>>> ObterInativasAsync()
        {
            var categorias = await _repository.ObterTodasCategoriasInativasAsync();
            var requests = _mapper.MapToDtos(categorias).ToList();

            return Resultado<ICollection<CategoriaRequest>>.Sucesso(requests);
        }

        public async Task<Resultado<CategoriaRequest>> ObterPorCodigoAsync(string codigo)
        {
            var categoria = await _repository.ObterCategoriaPorCodigoAsync(codigo);
            if (categoria == null)
            {
                var bag = new NotificationBag();
                bag.MensagemRegistroNaoEncontrado(codigo);
                return Resultado<CategoriaRequest>.Falhas(bag.Messages.ToList());
            }

            return Resultado<CategoriaRequest>.Sucesso(_mapper.MapToDto(categoria));
        }
    }
}
