using Application.DTO;
using Application.Interfaces.Services;
using Application.UseCases.CargoCases;
using Shared.Domain.ValueObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services.EntitiesServices
{
    public class CargoService(
        CriarCargoCase criarCargo,
        AtualizarCargoCase atualizarCargo,
        ExcluirCargoCase excluirCargo,
        ObterCargoCase obterCargo) : ICargoService
    {
        private readonly CriarCargoCase _criarCargo = criarCargo;
        private readonly AtualizarCargoCase _atualizarCargo = atualizarCargo;
        private readonly ExcluirCargoCase _excluirCargo = excluirCargo;
        private readonly ObterCargoCase _obterCargo = obterCargo;

        public Task<Resultado> CriarAsync(CargoDTO cargoDTO)
            => _criarCargo.ExecutarAsync(cargoDTO);

        public Task<Resultado> AtualizarAsync(string codigo, CargoDTO cargoDTO)
            => _atualizarCargo.ExecutarAsync(codigo, cargoDTO);

        public Task<Resultado> RemoverAsync(string codigo)
            => _excluirCargo.ExecutarAsync(codigo);

        public Task<Resultado<CargoDTO>> ObterPorCodigoAsync(string codigo)
            => _obterCargo.ObterPorCodigoAsync(codigo);

        public Task<Resultado<List<CargoDTO>>> ObterTodosAsync()
            => _obterCargo.ObterTodosAsync();
    }
}
