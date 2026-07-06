using Application.DTO;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
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
        private readonly IDepartamentoRepository _departamentoRepository;
        private readonly ICargoRepository _cargoRepository;
        private readonly IValidador<TarefaDTO> _validador;

        public TarefaPreparacaoService(
            IAsyncApplicationMapService<TarefaDTO, Tarefa> map,
            ITarefaEstadoRepository tarefaEstadoRepository,
            IUsuarioRepository usuarioRepository,
            IDepartamentoRepository departamentoRepository,
            ICargoRepository cargoRepository,
            IValidador<TarefaDTO> validador)
        {
            _map = map;
            _tarefaEstadoRepository = tarefaEstadoRepository;
            _usuarioRepository = usuarioRepository;
            _departamentoRepository = departamentoRepository;
            _cargoRepository = cargoRepository;
            _validador = validador;
        }

        public async Task<Resultado<TarefaPreparada>> PrepararParaPersistenciaAsync(TarefaDTO tarefaDTO)
        {
            if (tarefaDTO is not null && tarefaDTO.DestinoInicial == 0)
            {
                tarefaDTO.DestinoInicial = (int)DestinoInicialTarefa.Usuario;
            }

            var erros = _validador.Validar(tarefaDTO);
            if (erros.Any())
                return Resultado<TarefaPreparada>.Falhas(erros);

            var estado = await _tarefaEstadoRepository.ObterPorIdAsync(tarefaDTO.EstadoDaTarefa.Id);
            if (estado is null)
                return Resultado<TarefaPreparada>.Falha("Estado da tarefa nao encontrado.");

            tarefaDTO.EstadoDaTarefa = MapearEstado(estado);

            var tarefa = await _map.MapToEntityAsync(tarefaDTO);
            var usuario = await VincularUsuarioAsync(tarefa, tarefaDTO);
            if (usuario.TeveFalha)
                return Resultado<TarefaPreparada>.Falhas(usuario.Messages);

            var estrutura = await VincularEstruturaAsync(tarefa, tarefaDTO);
            if (estrutura.TeveFalha)
                return Resultado<TarefaPreparada>.Falhas(estrutura.Messages);

            return Resultado<TarefaPreparada>.Sucesso(new TarefaPreparada(tarefaDTO, tarefa, usuario.Dados));
        }

        public async Task<Resultado<List<TarefaEstadoDTO>>> ObterEstadosAsync()
        {
            var estados = await _tarefaEstadoRepository.ObterTodosAsync();
            return Resultado<List<TarefaEstadoDTO>>.Sucesso(estados.Select(MapearEstado).ToList());
        }

        private async Task<Resultado<Usuario>> VincularUsuarioAsync(Tarefa tarefa, TarefaDTO tarefaDTO)
        {
            if (tarefaDTO.UsuarioCodigo.IsNullOrEmpty())
            {
                tarefa.UsuarioId = null;
                tarefa.UsuarioCodigo = null;
                return Resultado<Usuario>.Sucesso(null);
            }

            var usuario = await _usuarioRepository.ObterUsuarioPorCodigoAsync(tarefaDTO.UsuarioCodigo.ToUpper());
            if (usuario is null)
                return Resultado<Usuario>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            tarefa.UsuarioId = usuario.Id;
            tarefa.UsuarioCodigo = usuario.Codigo;

            return Resultado<Usuario>.Sucesso(usuario);
        }

        private async Task<Resultado> VincularEstruturaAsync(Tarefa tarefa, TarefaDTO tarefaDTO)
        {
            if (tarefaDTO.DepartamentoCodigo.IsNullOrEmpty())
            {
                tarefa.DepartamentoId = null;
                tarefa.DepartamentoCodigo = null;
                tarefa.CargoId = null;
                tarefa.CargoCodigo = null;
                return Resultado.Sucesso();
            }

            var departamento = await _departamentoRepository
                .ObterDepartamentoPorCodigoRepositoryAsyncAsNoTracking(tarefaDTO.DepartamentoCodigo.ToUpper());
            if (departamento is null)
                return Resultado.Falha("Departamento da tarefa nao encontrado.");

            tarefa.DepartamentoId = departamento.Id;
            tarefa.DepartamentoCodigo = departamento.Codigo;

            if (tarefaDTO.CargoCodigo.IsNullOrEmpty())
            {
                tarefa.CargoId = null;
                tarefa.CargoCodigo = null;
                return Resultado.Sucesso();
            }

            var cargo = await _cargoRepository.ObterCargoPorCodigoAsync(tarefaDTO.CargoCodigo.ToUpper());
            if (cargo is null)
                return Resultado.Falha("Cargo da tarefa nao encontrado.");

            if (cargo.DepartamentoId != departamento.Id || cargo.DepartamentoCodigo != departamento.Codigo)
                return Resultado.Falha("Cargo informado nao pertence ao departamento da tarefa.");

            tarefa.CargoId = cargo.Id;
            tarefa.CargoCodigo = cargo.Codigo;

            return Resultado.Sucesso();
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
