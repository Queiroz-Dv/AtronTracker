using Application.DTO;
using Application.Mapping;
using Domain.Interfaces;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.UseCases.CargoCases
{
    public sealed class ObterCargoCase(
        CargoMapping mapper,
        ICargoRepository cargoRepository)
    {
        private readonly CargoMapping _mapper = mapper;
        private readonly ICargoRepository _cargoRepository = cargoRepository;

        public async Task<Resultado<CargoDTO>> ObterPorCodigoAsync(string codigo)
        {
            if (codigo.IsNullOrEmpty())
                return Resultado<CargoDTO>.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var cargo = await _cargoRepository.ObterCargoPorCodigoAsync(codigo);

            return cargo == null
                ? Resultado<CargoDTO>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado)
                : Resultado<CargoDTO>.Sucesso(_mapper.MapToDto(cargo));
        }

        public async Task<Resultado<List<CargoDTO>>> ObterTodosAsync()
        {
            var cargos = await _cargoRepository.ObterCargosAsync();
            var dtos = _mapper.MapToDtos(cargos).ToList();

            return Resultado<List<CargoDTO>>.Sucesso(dtos);
        }
    }
}
