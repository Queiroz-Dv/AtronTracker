using Application.DTO;
using Application.UseCases.UsuarioCases;
using Domain.Entities;
using Domain.Interfaces;
using Shared.Application.Interfaces.Mapping;
using Shared.Domain.ValueObjects;

namespace Application.UseCases.TarefaCases
{
    public class ObterSolicitacaoCase(
        IToDtoMapper<SolicitacaoObtencaoTarefa, SolicitacaoObtencaoTarefaDTO> mapper,
        ObterUsuarioCase usuarioService,
        IDepartamentoRepository departamentoRepository,
        ISolicitacaoObtencaoTarefaRepository solicitacaoRepository)
    {
        private readonly ObterUsuarioCase _usuarioService = usuarioService;
        private readonly IDepartamentoRepository _departamentoRepository = departamentoRepository;
        private readonly ISolicitacaoObtencaoTarefaRepository _repository = solicitacaoRepository;
        private readonly IToDtoMapper<SolicitacaoObtencaoTarefa, SolicitacaoObtencaoTarefaDTO> _mapper = mapper;

        public async Task<Resultado<IReadOnlyCollection<SolicitacaoObtencaoTarefaDTO>>> ExecutarAsync()
        {
            var usuarioResultado = await _usuarioService.ObterAsync();
            if (usuarioResultado.TeveFalha)
                return Resultado<IReadOnlyCollection<SolicitacaoObtencaoTarefaDTO>>.Falhas(usuarioResultado.Messages);

            var usuario = usuarioResultado.Dados!;

            var departamentosResultado = await _departamentoRepository.ObterDepartamentosPorCodigoGestorAsync(usuario.Codigo);

            var codigosDepartamentos = departamentosResultado?
                .Select(departamento => departamento.Codigo)
                .ToList() ?? [];

            var solicitacoes = await _repository.ObterPendentesPorAprovadorOuDepartamentosAsync(
                usuario.Id,
                usuario.Codigo,
                codigosDepartamentos);

            var dtos = _mapper.MapToDtos(solicitacoes).ToList();
            return Resultado<IReadOnlyCollection<SolicitacaoObtencaoTarefaDTO>>.Sucesso(dtos);
        }
    }
}