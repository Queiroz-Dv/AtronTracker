using Application.DTO;
using Application.Mapping;
using Domain.Interfaces;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.UseCases.DepartamentoCases
{
    public sealed class ObterDepartamentoCase(
        DepartamentoMapping mapper,
        IDepartamentoRepository departamentoRepository)
    {
        private readonly DepartamentoMapping _mapper = mapper;
        private readonly IDepartamentoRepository _departamentoRepository = departamentoRepository;

        public async Task<Resultado<List<DepartamentoDTO>>> ObterTodosAsync()
        {
            var entidades = await _departamentoRepository.ObterDepartmentosAsync();
            var departamentos = _mapper.MapToDtos(entidades).ToList();

            return Resultado<List<DepartamentoDTO>>.Sucesso(departamentos);
        }

        public async Task<Resultado<DepartamentoDTO>> ObterPorCodigoAsync(string codigo)
        {
            if (codigo.IsNullOrEmpty())
                return Resultado<DepartamentoDTO>.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var departamento = await _departamentoRepository
                .ObterDepartamentoPorCodigoRepositoryAsync(codigo);

            return departamento is null
                ? Resultado<DepartamentoDTO>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado)
                : Resultado<DepartamentoDTO>.Sucesso(_mapper.MapToDto(departamento));
        }

        public async Task<Resultado<DepartamentoDTO>> ObterPorIdAsync(int? departamentoId)
        {
            if (departamentoId == 0)
                return Resultado<DepartamentoDTO>.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var departamento = await _departamentoRepository
                .ObterDepartamentoPorIdRepositoryAsync(departamentoId);

            return departamento is null
                ? Resultado<DepartamentoDTO>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado)
                : Resultado<DepartamentoDTO>.Sucesso(_mapper.MapToDto(departamento));
        }

        public async Task<Resultado<IEnumerable<DepartamentoDTO>>> ObterPorGestorAsync(string usuarioCodigo)
        {
            if (usuarioCodigo.IsNullOrEmpty())
                return Resultado<IEnumerable<DepartamentoDTO>>.Sucesso([]);

            var entidades = await _departamentoRepository
                .ObterDepartamentosPorCodigoGestorAsync(usuarioCodigo);

            return Resultado<IEnumerable<DepartamentoDTO>>
                .Sucesso(_mapper.MapToDtos(entidades));
        }
    }
}
