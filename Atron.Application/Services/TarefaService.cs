using Atron.Application.DTO;
using Atron.Application.Interfaces;
using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using AutoMapper;
using Notification.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.Application.Services
{
    public class TarefaService : ITarefaService
    {
        private readonly IMapper _mapper;
        private readonly ICargoRepository _cargoRepository;
        private readonly IDepartamentoRepository _departamentoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUsuarioService _usuarioService;
        private readonly ITarefaRepository _tarefaRepository;
        private readonly ITarefaEstadoRepository _tarefaEstadoRepository;

        private readonly NotificationModel<Tarefa> _notification;

        public List<NotificationMessage> Messages { get; set; }

        public TarefaService(IMapper mapper,
                             ICargoRepository cargoRepository,
                             IDepartamentoRepository departamentoRepository,
                             IUsuarioRepository usuarioRepository,
                             IUsuarioService usuarioService,
                             ITarefaRepository tarefaRepository,
                             ITarefaEstadoRepository tarefaEstadoRepository,
                             NotificationModel<Tarefa> notification)
        {
            _mapper = mapper;
            _notification = notification;
            _cargoRepository = cargoRepository;
            _departamentoRepository = departamentoRepository;
            _usuarioRepository = usuarioRepository;
            _usuarioService = usuarioService;
            _tarefaRepository = tarefaRepository;
            _tarefaEstadoRepository = tarefaEstadoRepository;
            Messages = new List<NotificationMessage>();
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
                await _tarefaRepository.CriarTarefaAsync(tarefa);
                Messages.Add(new NotificationMessage("Tarefa criada com sucesso."));
            }
        }

        public async Task<List<TarefaDTO>> ObterTodosAsync()
        {
            var tarefas = await _tarefaRepository.ObterTarefasAsync();
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
    }
}