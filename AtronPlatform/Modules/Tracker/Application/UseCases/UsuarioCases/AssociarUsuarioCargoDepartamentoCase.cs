using Application.DTO.Request;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.UsuarioInterfaces;
using System.Threading.Tasks;

namespace Application.UseCases.UsuarioCases
{
    public sealed class AssociarUsuarioCargoDepartamentoCase(
        IDepartamentoRepository departamentoRepository,
        ICargoRepository cargoRepository,
        IUsuarioCargoDepartamentoRepository usuarioCargoDepartamentoRepository
        )
    {
        private readonly IDepartamentoRepository _departamentoRepository = departamentoRepository;
        private readonly ICargoRepository _cargoRepository = cargoRepository;
        private readonly IUsuarioCargoDepartamentoRepository _usuarioCargoDepartamentoRepository = usuarioCargoDepartamentoRepository;

        public async Task ExecutarAsync(UsuarioRequest request, Usuario usuario)
        {
            var departamento = await _departamentoRepository.ObterDepartamentoPorCodigoRepository(request.DepartamentoCodigo);
            var cargo = await _cargoRepository.ObterCargoPorCodigoAsync(request.CargoCodigo);

            if (departamento != null && cargo != null)
            {
                await _usuarioCargoDepartamentoRepository.GravarAssociacaoUsuarioCargoDepartamento(usuario, cargo, departamento);
            }
        }
    }
}