using Application.DTO;
using Application.Mapping;
using Domain.Interfaces;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using System.Linq;
using System.Threading.Tasks;

namespace Application.UseCases.DepartamentoCases
{
    public sealed class CriarDepartamentoCase(
        VincularGestorDepartamentoCase vincularGestorDepartamento,
        DepartamentoMapping mapper,
        IDepartamentoRepository repository,
        IValidador<DepartamentoDTO> validador)
    {
        private readonly VincularGestorDepartamentoCase _vincularGestorDepartamento = vincularGestorDepartamento;
        private readonly DepartamentoMapping _mapper = mapper;
        private readonly IDepartamentoRepository _departamentoRepository = repository;
        private readonly IValidador<DepartamentoDTO> _validador = validador;

        public async Task<Resultado> ExecutarAsync(DepartamentoDTO departamentoDTO)
        {
            var erros = _validador.Validar(departamentoDTO);
            if (erros.Any())
                return Resultado<DepartamentoDTO>.Falhas(erros);

            var departamentoExiste = await _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsync(departamentoDTO.Codigo);
            if (departamentoExiste != null)
                return Resultado<DepartamentoDTO>.Falha(DepartamentoResource.ErroCodigoDepartamentoExistente);

            var departamento = _mapper.MapToEntity(departamentoDTO);

            var resultadoGestor = await _vincularGestorDepartamento
                .ExecutarAsync(departamento, departamentoDTO.GestorDepartamentoCodigo);

            if (resultadoGestor.TeveFalha)
                return Resultado<DepartamentoDTO>.Falhas(resultadoGestor.Messages);

            var foiCriado = await _departamentoRepository.CriarDepartamentoRepositoryAsync(departamento);
            if (!foiCriado)
                return Resultado<DepartamentoDTO>.Falha(DepartamentoResource.ErroGravacao);

            return Resultado<DepartamentoDTO>
                .Sucesso(departamentoDTO)
                .ComMensagemRegistroSalvo(departamento.Codigo);
        }
    }
}
