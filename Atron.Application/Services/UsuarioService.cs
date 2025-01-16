using Atron.Application.DTO;
using Atron.Application.Interfaces;
using Atron.Domain.ApiEntities;
using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Atron.Domain.Interfaces.ApplicationInterfaces;
using Atron.Domain.Interfaces.UsuarioInterfaces;
using AutoMapper;
using Shared.Extensions;
using Shared.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        // Sempre usar o repository pra acessar a camada de dados
        private readonly IMapper _mapper;
        private readonly IRegisterApplicationRepository _registerApplicationRepository;
        private readonly IUsuarioCargoDepartamentoRepository _usuarioCargoDepartamentoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ICargoRepository _cargoRepository;
        private readonly IDepartamentoRepository _departamentoRepository;
        private readonly MessageModel<Usuario> _messageModel;

        public bool Registrar { get; set; }

        public UsuarioService(IMapper mapper,
                              IRegisterApplicationRepository registerApplicationRepository,
                              IUsuarioCargoDepartamentoRepository usuarioCargoDepartamentoRepository,
                              IUsuarioRepository repository,
                              ICargoRepository cargoRepository,
                              IDepartamentoRepository departamentoRepository,
                              MessageModel<Usuario> messageModel)
        {
            _mapper = mapper;
            _registerApplicationRepository = registerApplicationRepository;
            _usuarioCargoDepartamentoRepository = usuarioCargoDepartamentoRepository;
            _usuarioRepository = repository;
            _cargoRepository = cargoRepository;
            _departamentoRepository = departamentoRepository;
            _messageModel = messageModel;
        }

        public async Task AtualizarAsync(UsuarioDTO usuarioDTO)
        {
            if (usuarioDTO is null)
            {
                _messageModel.AddRegisterInvalidMessage(nameof(Usuario));
                return;
            }

            var entidade = new Usuario()
            {
                Id = usuarioDTO.Id,
                IdSequencial = usuarioDTO.IdSequencial,
                Codigo = usuarioDTO.Codigo,
                Nome = usuarioDTO.Nome,
                Sobrenome = usuarioDTO.Sobrenome,
                DataNascimento = usuarioDTO.DataNascimento,
                SalarioAtual = usuarioDTO.Salario,
            };

            // await MontarEntidadesComplementares(usuarioDTO, entidade);
            _messageModel.Validate(entidade);

            if (!_messageModel.Messages.HasErrors())
            {
                await _usuarioRepository.AtualizarUsuarioAsync(entidade);
                _messageModel.AddUpdateMessage(nameof(Usuario));
            }
        }

        public async Task<bool> CriarAsync(UsuarioDTO usuarioDTO)
        {
            if (usuarioDTO is null)
            {
                _messageModel.AddRegisterInvalidMessage(nameof(Cargo));
                return false;
            }

            var usuario = new Usuario()
            {
                Id = usuarioDTO.Id,
                IdSequencial = usuarioDTO.IdSequencial,
                Codigo = usuarioDTO.Codigo,
                Nome = usuarioDTO.Nome,
                Sobrenome = usuarioDTO.Sobrenome,
                DataNascimento = usuarioDTO.DataNascimento,
                SalarioAtual = usuarioDTO.Salario,
            };

            var cargo = new Cargo();
            var departamento = new Departamento();

            if (!usuarioDTO.CargoCodigo.IsNullOrEmpty() && !usuarioDTO.DepartamentoCodigo.IsNullOrEmpty())
            {
                var departamentoBd = await _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsync(usuarioDTO.DepartamentoCodigo);
                var cargoBd = await _cargoRepository.ObterCargoPorCodigoAsync(usuarioDTO.CargoCodigo);

                departamento.Id = departamentoBd.Id;
                departamento.Codigo = departamentoBd.Codigo;
                cargo.Id = cargoBd.Id;
                cargo.Codigo = cargoBd.Codigo;
            }


            // Usar specification e validation
            _messageModel.Validate(usuario);
            if (!_messageModel.Messages.HasErrors())
            {
                var usr = await _usuarioRepository.CriarUsuarioAsync(usuario);
                if (usr)
                {
                    if (!cargo.Codigo.IsNullOrEmpty() && !departamento.Codigo.IsNullOrEmpty())
                    {
                        await _usuarioCargoDepartamentoRepository.GravarAssociacaoUsuarioCargoDepartamento(usuario, cargo, departamento);
                    }

                    _messageModel.AddSuccessMessage(nameof(Usuario));

                    if (Registrar)
                    {
                        var register = new ApiRegister()
                        {
                            UserName = usuario.Nome,
                            Email = usuario.Email,
                            Password = usuarioDTO.Senha
                        };

                        var registerOk = await _registerApplicationRepository.RegisterUserAccountAsync(register);

                        if (registerOk)
                        {
                            _messageModel.AddMessage($"Usuário de acesso da aplicação: {usuarioDTO.Nome} cadastrado com sucesso");
                        }
                    }
                }
            }

            return false;
        }

        public async Task<UsuarioDTO> ObterPorCodigoAsync(string codigo)
        {
            var usuario = await _usuarioRepository.ObterUsuarioPorCodigoAsync(codigo);

            var usuarioDTO = new UsuarioDTO()
            {
                Id = usuario.Id,
                Codigo = usuario.Codigo,
                Nome = usuario.Nome,
                Sobrenome = usuario.Sobrenome,
                Salario = usuario.SalarioAtual,
                DataNascimento = usuario.DataNascimento,
                Email = usuario.Email,
                //CargoCodigo = usuario.CargoCodigo,
                //DepartamentoCodigo = usuario.DepartamentoCodigo,
                //Cargo = new CargoDTO() { Codigo = usuario.Cargo.Codigo, Descricao = usuario.Cargo.Descricao },
                //Departamento = new DepartamentoDTO() { Codigo = usuario.Departamento.Codigo, Descricao = usuario.Departamento.Descricao },
            };

            return usuarioDTO;
        }

        public async Task<List<UsuarioDTO>> ObterTodosAsync()
        {
            var usuarios = await _usuarioRepository.ObterUsuariosAsync();
            var usuariosDTOs = _mapper.Map<IEnumerable<UsuarioDTO>>(usuarios);

            var usuarioPreenchido = (from usr in usuariosDTOs
                                     select new UsuarioDTO
                                     {
                                         Id = usr.Id,
                                         Codigo = usr.Codigo,
                                         Nome = usr.Nome,
                                         Sobrenome = usr.Sobrenome,
                                         Salario = usr.Salario,
                                         DataNascimento = usr.DataNascimento,
                                         CargoCodigo = usr.CargoCodigo,
                                         DepartamentoCodigo = usr.DepartamentoCodigo,
                                         Cargo = new CargoDTO()
                                         {
                                             Codigo = usr.Cargo.Codigo,
                                             Descricao = usr.Cargo.Descricao,
                                         },
                                         Departamento = new DepartamentoDTO()
                                         {
                                             Codigo = usr.Departamento.Codigo,
                                             Descricao = usr.Departamento.Descricao
                                         },
                                     }).ToList();

            return usuarioPreenchido;
        }

        public async Task RemoverAsync(string codigo)
        {
            var usuario = await _usuarioRepository.ObterUsuarioPorCodigoAsync(codigo);

            if (usuario == null)
            {
                _messageModel.AddRegisterNotFoundMessage(nameof(Usuario));
            }
            else
            {
                await _usuarioRepository.RemoverUsuarioAsync(usuario);
                _messageModel.AddRegisterRemovedSuccessMessage(nameof(Usuario));
            }
        }
    }
}