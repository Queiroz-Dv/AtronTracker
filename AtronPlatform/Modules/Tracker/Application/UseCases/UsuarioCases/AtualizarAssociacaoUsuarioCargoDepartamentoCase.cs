using Application.DTO.Request;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Threading.Tasks;

namespace Application.UseCases.UsuarioCases
{
    public sealed class AtualizarAssociacaoUsuarioCargoDepartamentoCase(
        IDepartamentoRepository departamentoRepository,
        ICargoRepository cargoRepository,
        IUsuarioCargoDepartamentoRepository usuarioCargoDepartamentoRepository)
    {
        private readonly IDepartamentoRepository _departamentoRepository = departamentoRepository;
        private readonly ICargoRepository _cargoRepository = cargoRepository;
        private readonly IUsuarioCargoDepartamentoRepository _usuarioCargoDepartamentoRepository = usuarioCargoDepartamentoRepository;

        public async Task<Resultado> ExecutarAsync(UsuarioRequest request, Usuario usuario)
        {
            if (request.DepartamentoCodigo.IsNullOrEmpty() || request.CargoCodigo.IsNullOrEmpty())
                return Resultado.Sucesso();

            var departamento = await _departamentoRepository
                .ObterDepartamentoPorCodigoRepository(request.DepartamentoCodigo);
            var cargo = await _cargoRepository.ObterCargoPorCodigoAsync(request.CargoCodigo);

            if (departamento is null || cargo is null)
                return Resultado.Sucesso();

            var relacionamentoExistente = await _usuarioCargoDepartamentoRepository
                .ObterPorChaveDoUsuario(usuario.Id, usuario.Codigo);

            if (RelacionamentoJaAtualizado(relacionamentoExistente, cargo, departamento))
                return Resultado.Sucesso();

            if (relacionamentoExistente is not null)
            {
                var removido = await _usuarioCargoDepartamentoRepository
                    .RemoverRepositoryAsync(relacionamentoExistente);
                if (!removido)
                    return Resultado.Falha(UsuarioResource.ErroInesperadoAtualizacao);
            }

            var gravado = await _usuarioCargoDepartamentoRepository
                .GravarAssociacaoUsuarioCargoDepartamento(usuario, cargo, departamento);

            return gravado
                ? Resultado.Sucesso()
                : Resultado.Falha(UsuarioResource.ErroInesperadoAtualizacao);
        }

        private static bool RelacionamentoJaAtualizado(
            UsuarioCargoDepartamento relacionamento,
            Cargo cargo,
            Departamento departamento)
        {
            return relacionamento is not null &&
                   relacionamento.CargoId == cargo.Id &&
                   relacionamento.CargoCodigo == cargo.Codigo &&
                   relacionamento.DepartamentoId == departamento.Id &&
                   relacionamento.DepartamentoCodigo == departamento.Codigo;
        }
    }
}
