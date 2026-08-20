using Application.DTO;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Interfaces;
using Shared.Application.Interfaces.Mapping;
using Shared.Domain.ValueObjects;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.UseCases.TarefaCases
{
    public class ObterSolicitacaoCase(
        IToDtoMapper<SolicitacaoObtencaoTarefa, SolicitacaoObtencaoTarefaDTO> mapper,
        IUsuarioService usuarioService, 
        IDepartamentoService departamentoService,
        ISolicitacaoObtencaoTarefaRepository solicitacaoRepository)
    {
        private readonly IUsuarioService _usuarioService = usuarioService;
        private readonly IDepartamentoService _departamentoService = departamentoService;
        private readonly ISolicitacaoObtencaoTarefaRepository _repository = solicitacaoRepository;
        private readonly IToDtoMapper<SolicitacaoObtencaoTarefa, SolicitacaoObtencaoTarefaDTO> _mapper = mapper;

        public async Task<Resultado<IReadOnlyCollection<SolicitacaoObtencaoTarefaDTO>>> ExecutarAsync()
        {
            var usuarioResultado = await _usuarioService.ObterUsuarioAtual();
            if (usuarioResultado.TeveFalha)
                return Resultado<IReadOnlyCollection<SolicitacaoObtencaoTarefaDTO>>.Falhas(usuarioResultado.Messages);

            var usuario = usuarioResultado.Dados!;

            var departamentosResultado = await _departamentoService.ObterDepartamentosPorGestor(usuario.Codigo);
            if (departamentosResultado.TeveFalha)
                return Resultado<IReadOnlyCollection<SolicitacaoObtencaoTarefaDTO>>.Falhas(departamentosResultado.Messages);

            var codigosDepartamentos = departamentosResultado.Dados?
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
