using Atron.Application.DTO;
using Atron.Application.Interfaces;
using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using AutoMapper;
using Notification.Models;
using Shared.DTO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.Application.Services
{
    public class TarefaService : ITarefaService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Tarefa> _repository;
        private readonly ICargoRepository _cargoRepository;
        private readonly IDepartamentoRepository _departamentoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUsuarioService _usuarioService;
        private readonly ITarefaRepository _tarefaRepository;
        private readonly ITarefaEstadoRepository _tarefaEstadoRepository;

        private readonly NotificationModel<Tarefa> _notification;

        public List<NotificationMessage> _messages { get; set; }  

        public List<ResultResponse> ResultApiJson => throw new System.NotImplementedException();

        public TarefaService(IMapper mapper,
                             IRepository<Tarefa> repository,
                             ICargoRepository cargoRepository,
                             IDepartamentoRepository departamentoRepository,
                             IUsuarioRepository usuarioRepository,
                             IUsuarioService usuarioService,
                             ITarefaRepository tarefaRepository,
                             ITarefaEstadoRepository tarefaEstadoRepository,
                             NotificationModel<Tarefa> notification)
        {
            _mapper = mapper;
            _repository = repository;
            _notification = notification;
            _cargoRepository = cargoRepository;
            _departamentoRepository = departamentoRepository;
            _usuarioRepository = usuarioRepository;
            _usuarioService = usuarioService;
            _tarefaRepository = tarefaRepository;
            _tarefaEstadoRepository = tarefaEstadoRepository;
            _messages = new List<NotificationMessage>();
        }

        public async Task CriarAsync(TarefaDTO tarefaDTO)
        {
            tarefaDTO.Id = tarefaDTO.GerarIdentificador();

            var usuario = await _usuarioRepository.ObterUsuarioPorCodigoAsync(tarefaDTO.UsuarioCodigo);
            if (usuario is not null)
            {
                tarefaDTO.UsuarioId = usuario.Id;
            }

            var tarefa = _mapper.Map<Tarefa>(tarefaDTO);

            _notification.Validate(tarefa);

            if (!_notification.Messages.HasErrors())
            {
                await _tarefaRepository.CriarRepositoryAsync(tarefa);
                _messages.Add(new NotificationMessage("Tarefa criada com sucesso."));
            }
        }

        public async Task<List<TarefaDTO>> ObterTodosAsync()
        {
            var tarefas = await _tarefaRepository.ObterTodosRepositoryAsync();
            var estadosDaTarefa = await _tarefaEstadoRepository.ObterTodosAsync();
            var usuarios = await _usuarioService.ObterTodosAsync();

            var tarefasDTO = _mapper.Map<IEnumerable<TarefaDTO>>(tarefas);
            var tarefasDTOList = new List<TarefaDTO>();

            foreach (var tarefaDTO in tarefasDTO)
            {
                foreach (var usuario in usuarios)
                {
                    if (tarefaDTO.UsuarioId == usuario.Id)
                    {
                        var tarefa = new TarefaDTO();
                        tarefa.UsuarioCodigo = tarefaDTO.UsuarioCodigo;

                        tarefa.Usuario = new UsuarioDTO()
                        {
                            Codigo = usuario.Codigo,
                            Nome = usuario.Nome,
                            Sobrenome = usuario.Sobrenome,
                            DataNascimento = usuario.DataNascimento,
                            Salario = usuario.Salario,
                            CargoCodigo = usuario.CargoCodigo,
                            DepartamentoCodigo = usuario.DepartamentoCodigo,

                            Departamento = new DepartamentoDTO()
                            {
                                Codigo = usuario.Departamento.Codigo,
                                Descricao = usuario.Departamento.Descricao,
                            },

                            Cargo = new CargoDTO()
                            {
                                Codigo = usuario.Cargo.Codigo,
                                Descricao = usuario.Cargo.Descricao,
                            }
                        };

                        tarefa.Titulo = tarefaDTO.Titulo;
                        tarefa.Conteudo = tarefaDTO.Conteudo;
                        tarefa.DataInicial = tarefaDTO.DataInicial.Date;
                        tarefa.DataFinal = tarefaDTO.DataFinal.Date;
                        tarefa.EstadoDaTarefa = tarefaDTO.EstadoDaTarefa;
                        tarefa.EstadoDaTarefaDescricao = estadosDaTarefa.FirstOrDefault(tre => tre.Id == tarefa.EstadoDaTarefa).Descricao;

                        tarefasDTOList.Add(tarefa);
                    }
                }
            }
            return tarefasDTOList;
        }

        public async Task AtualizarAsync(TarefaDTO tarefaDTO)
        {
            var tarefa = _mapper.Map<Tarefa>(tarefaDTO);

            var usuarioExiste = _usuarioRepository.UsuarioExiste(tarefa.UsuarioCodigo);

            if (usuarioExiste)
            {
                var usuario = await _usuarioRepository.ObterUsuarioPorCodigoAsync(tarefa.UsuarioCodigo);
                var tarefaEstados = await _tarefaEstadoRepository.ObterTodosAsync();

                tarefa.UsuarioId = usuario.Id;
                tarefa.UsuarioCodigo = usuario.Codigo;
                tarefa.EstadoDaTarefa = tarefaEstados.FirstOrDefault(tre => tre.Id == tarefa.EstadoDaTarefa).Id;

                _notification.Validate(tarefa);

                if (!_notification.Messages.HasErrors())
                {
                    await _tarefaRepository.AtualizarRepositoryAsync(tarefa);
                    _messages.Add(new NotificationMessage("Tarefa atualizada com sucesso."));
                    return;
                }

                _messages.AddRange(_notification.Messages);
            }
            else
            {
                _messages.Add(new NotificationMessage("Código de usuário não existe. Tente novamente", Notification.Enums.ENotificationType.Error));
                return;
            }
        }

        public async Task ExcluirAsync(int id)
        {
            var tarefa = await _tarefaRepository.ObterPorIdRepositoryAsync(id);

            if (tarefa is null)
            {
                _messages.Add(new NotificationMessage("Tarefa não existe. Tente novamente"));
                return;
            }

            await _repository.RemoverRepositoryAsync(tarefa);
            _messages.Add(new NotificationMessage("Registro removido com sucesso"));
        }
    }
}