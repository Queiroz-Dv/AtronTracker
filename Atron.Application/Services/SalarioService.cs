using Atron.Application.DTO;
using Atron.Application.Interfaces;
using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using AutoMapper;
using Notification.Models;
using Shared.Extensions;
using Shared.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.Application.Services
{
    public class SalarioService : ISalarioService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Salario> _repository;
        private readonly ICargoRepository _cargoRepository;
        private readonly IDepartamentoRepository _departamentoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUsuarioService _usuarioService;
        private readonly ITarefaRepository _tarefaRepository;
        private readonly ITarefaEstadoRepository _tarefaEstadoRepository;
        private readonly ISalarioRepository _salarioRepository;
        private readonly IMesRepository _mesRepository;
        private readonly MessageModel<Salario> _messageModel;

        public SalarioService(IMapper mapper,
                              IRepository<Salario> repository,
                              ICargoRepository cargoRepository,
                              IDepartamentoRepository departamentoRepository,
                              IUsuarioRepository usuarioRepository,
                              IUsuarioService usuarioService,
                              ITarefaRepository tarefaRepository,
                              ITarefaEstadoRepository tarefaEstadoRepository,
                              ISalarioRepository salarioRepository,
                              IMesRepository mesRepository,
                              MessageModel<Salario> messageModel)
        {
            _mapper = mapper;
            _repository = repository;
            _cargoRepository = cargoRepository;
            _departamentoRepository = departamentoRepository;
            _usuarioRepository = usuarioRepository;
            _usuarioService = usuarioService;
            _tarefaRepository = tarefaRepository;
            _tarefaEstadoRepository = tarefaEstadoRepository;
            _salarioRepository = salarioRepository;
            _mesRepository = mesRepository;
            _messageModel = messageModel;
        }

        public async Task AtualizarServiceAsync(SalarioDTO salarioDTO)
        {
            var entidade = _mapper.Map<Salario>(salarioDTO);

            var usuario = await _usuarioRepository.ObterUsuarioPorCodigoAsync(entidade.UsuarioCodigo);
            var meses = await _mesRepository.ObterMesesRepositoryAsync();

            entidade.UsuarioId = usuario.Id;
            entidade.UsuarioCodigo = usuario.Codigo;
            entidade.MesId = salarioDTO.MesId;

            _messageModel.Validate(entidade);

            if (!_messageModel.Messages.HasErrors())
            {
                await AtualizarSalarioUsuario(entidade, usuario);
                await _salarioRepository.AtualizarSalarioRepositoryAsync(entidade);
                _messageModel.AddUpdateMessage(nameof(Salario));
                return;
            }
        }

        // Sempre que o meu salário for maior do que o cadastrado em usuário ele será atualizado
        private async Task AtualizarSalarioUsuario(Salario entidade, Usuario usuario)
        {
            if (entidade.SalarioMensal > usuario.SalarioAtual)
            {
                await _usuarioRepository.AtualizarSalario(entidade.UsuarioId, entidade.SalarioMensal);
            }
        }

        public async Task CriarAsync(SalarioDTO salarioDTO)
        {
            var usuario = await _usuarioRepository.ObterUsuarioPorCodigoAsync(salarioDTO.UsuarioCodigo);

            var entidade = new Salario()
            {
                IdSequencial = salarioDTO.IdSequencial,
                SalarioMensal = salarioDTO.SalarioMensal,
                Ano = salarioDTO.Ano,
                MesId = salarioDTO.MesId,
                UsuarioId = usuario.Id,
                UsuarioCodigo = salarioDTO.UsuarioCodigo,
            };

            _messageModel.Validate(entidade);

            if (!_messageModel.Messages.HasErrors())
            {
                await AtualizarSalarioUsuario(entidade, usuario);
                await _salarioRepository.CriarSalarioAsync(entidade);
                _messageModel.AddSuccessMessage(nameof(Salario));
            }
        }

        public Task ExcluirAsync(string id)
        {
            throw new System.NotImplementedException();
        }

        public async Task ExcluirServiceAsync(int id)
        {
            var salario = await _salarioRepository.ObterPorIdRepositoryAsync(id);

            if (salario is not null)
            {
                await _salarioRepository.RemoverRepositoryAsync(salario);
                //Messages.Add(new NotificationMessage("Registro removido com sucesso."));
            }
        }

        public async Task<List<MesDTO>> ObterMeses()
        {
            var mesesRepo = await _mesRepository.ObterMesesRepositoryAsync();

            var mesesDTO = _mapper.Map<List<MesDTO>>(mesesRepo);
            return mesesDTO;
        }

        public async Task<SalarioDTO> ObterPorId(int id)
        {
            var entidade = await _salarioRepository.ObterSalarioPorIdAsync(id);
            var salario = new SalarioDTO()
            {
                Id = entidade.Id,
                Ano = entidade.Ano,
                SalarioMensal = entidade.SalarioMensal,
                MesId = entidade.MesId,
                UsuarioCodigo = entidade.UsuarioCodigo,
                Usuario = new UsuarioDTO()
                {
                    Codigo = entidade.Usuario.Codigo,
                    Nome = entidade.Usuario.Nome,
                    Sobrenome = entidade.Usuario.Sobrenome,

                    Departamento = new DepartamentoDTO()
                    {
                        Codigo = entidade.Usuario.Departamento.Codigo,
                        Descricao = entidade.Usuario.Departamento.Descricao
                    },

                    Cargo = new CargoDTO()
                    {
                        Codigo = entidade.Usuario.Cargo.Codigo,
                        Descricao = entidade.Usuario.Cargo.Descricao,
                    }
                }
            };

            return salario;
        }

        public async Task<List<SalarioDTO>> ObterTodosAsync()
        {
            var usuariosDTO = await _usuarioService.ObterTodosAsync();
            var mesesRepository = await _mesRepository.ObterMesesRepositoryAsync();
            var salariosRepository = await _salarioRepository.ObterSalariosRepository();

            var salariosDTO = _mapper.Map<IEnumerable<SalarioDTO>>(salariosRepository);

            var salarios = salariosDTO
                          // Faz a junção da tabela de usuários com salários             
                          .Join(usuariosDTO,
                                salario => salario.Usuario.Id, // FK de salario
                                usuario => usuario.Id, // PK de usuário
                                (salario, usuario) => new { salario, usuario }) // resultado 

                          // Faz a junção da tabela de meses com salários
                          .Join(mesesRepository,
                                slr => slr.salario.Mes.Id, // FK de mês
                                mes => mes.Id, // PK de mês
                                               // 
                                               // Monta a entidade completa
                                (slr, mes) => new SalarioDTO
                                {
                                    Id = slr.salario.Id,
                                    Usuario = new UsuarioDTO()
                                    {
                                        Codigo = slr.usuario.Codigo,
                                        Nome = slr.usuario.Nome,
                                        Sobrenome = slr.usuario.Sobrenome,
                                        DataNascimento = slr.usuario.DataNascimento,
                                        Salario = slr.usuario.Salario,

                                        Departamento = new DepartamentoDTO()
                                        {
                                            Codigo = slr.usuario.Departamento.Codigo,
                                            Descricao = slr.usuario.Departamento.Descricao
                                        },

                                        Cargo = new CargoDTO()
                                        {
                                            Codigo = slr.usuario.Cargo.Codigo,
                                            Descricao = slr.usuario.Cargo.Descricao,
                                            DepartamentoCodigo = slr.usuario.Cargo.DepartamentoCodigo,
                                            DepartamentoDescricao = slr.usuario.Cargo.DepartamentoDescricao,
                                        }
                                    },

                                    Mes = new MesDTO()
                                    {
                                        Id = mes.Id,
                                        Descricao = mes.Descricao
                                    },
                                    Ano = slr.salario.Ano,
                                    SalarioMensal = slr.salario.SalarioMensal
                                }).OrderByDescending(slr => slr.Ano).ToList();

            return salarios;
        }
    }
}