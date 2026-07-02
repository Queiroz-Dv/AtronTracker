using Application.DTO;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services.EntitiesServices.Tarefas
{
    public class TarefaPreparacaoService : ITarefaPreparacaoService
    {
        private readonly IAsyncApplicationMapService<TarefaDTO, Tarefa> _map;
        private readonly ITarefaEstadoRepository _tarefaEstadoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IValidador<TarefaDTO> _validador;

        public TarefaPreparacaoService(
            IAsyncApplicationMapService<TarefaDTO, Tarefa> map,
            ITarefaEstadoRepository tarefaEstadoRepository,
            IUsuarioRepository usuarioRepository,
            IValidador<TarefaDTO> validador)
        {
            _map = map;
            _tarefaEstadoRepository = tarefaEstadoRepository;
            _usuarioRepository = usuarioRepository;
            _validador = validador;
        }

        public async Task<Resultado<TarefaPreparada>> PrepararParaPersistenciaAsync(TarefaDTO tarefaDTO)
        {
            var erros = _validador.Validar(tarefaDTO);
            if (erros.Any())
                return Resultado<TarefaPreparada>.Falhas(erros);

            var usuario = await _usuarioRepository.ObterUsuarioPorCodigoAsync(tarefaDTO.UsuarioCodigo.ToUpper());
            if (usuario is null)
                return Resultado<TarefaPreparada>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            var estado = await _tarefaEstadoRepository.ObterPorIdAsync(tarefaDTO.EstadoDaTarefa.Id);
            if (estado is null)
                return Resultado<TarefaPreparada>.Falha("Estado da tarefa nao encontrado.");

            tarefaDTO.EstadoDaTarefa = MapearEstado(estado);

            var tarefa = await _map.MapToEntityAsync(tarefaDTO);
            VincularUsuario(tarefa, usuario);

            return Resultado<TarefaPreparada>.Sucesso(new TarefaPreparada(tarefaDTO, tarefa, usuario));
        }

        public async Task<Resultado<List<TarefaEstadoDTO>>> ObterEstadosAsync()
        {
            var estados = await _tarefaEstadoRepository.ObterTodosAsync();
            return Resultado<List<TarefaEstadoDTO>>.Sucesso(estados.Select(MapearEstado).ToList());
        }

        private static void VincularUsuario(Tarefa tarefa, Usuario usuario)
        {
            tarefa.UsuarioId = usuario.Id;
            tarefa.UsuarioCodigo = usuario.Codigo;
        }

        private static TarefaEstadoDTO MapearEstado(TarefaEstado estado)
        {
            return new TarefaEstadoDTO
            {
                Id = estado.Id,
                Descricao = estado.Descricao
            };
        }
    }
}
