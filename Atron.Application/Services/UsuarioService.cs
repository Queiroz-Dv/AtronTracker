using Atron.Application.DTO;
using Atron.Application.Interfaces;
using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using AutoMapper;
using Notification.Models;
using Shared.Models;
using Shared.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        // Sempre usar o repository pra acessar a camada de dados
        private readonly IMapper _mapper;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ICargoRepository _cargoRepository;
        private readonly IDepartamentoRepository _departamentoRepository;
        private readonly MessageModel<Usuario> _messageModel;

        public UsuarioService(IMapper mapper,
                              IUsuarioRepository repository,
                              ICargoRepository cargoRepository,
                              IDepartamentoRepository departamentoRepository,
                              MessageModel<Usuario> messageModel)
        {
            _mapper = mapper;
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

            await MontarEntidadesComplementares(usuarioDTO, entidade);
            _messageModel.Validate(entidade);

            if (!_messageModel.Messages.HasErrors())
            {
                await _usuarioRepository.AtualizarUsuarioAsync(entidade);
                _messageModel.AddUpdateMessage(nameof(Usuario));
            }
        }

        public async Task CriarAsync(UsuarioDTO usuarioDTO)
        {
            if (usuarioDTO is null)
            {
                _messageModel.AddRegisterInvalidMessage(nameof(Cargo));
                return;
            }

            var usuario = new Usuario()
            {
                Id = usuarioDTO.Id,
                IdSequencial = usuarioDTO.IdSequencial,
                //IdSequencial = usua
                Codigo = usuarioDTO.Codigo,
                Nome = usuarioDTO.Nome,
                Sobrenome = usuarioDTO.Sobrenome,
                DataNascimento = usuarioDTO.DataNascimento,
                SalarioAtual = usuarioDTO.Salario,
            };

            await MontarEntidadesComplementares(usuarioDTO, usuario);
            _messageModel.Validate(usuario);

            if (!_messageModel.Messages.HasErrors())
            {
                await _usuarioRepository.CriarUsuarioAsync(usuario);
                _messageModel.AddSuccessMessage(nameof(Usuario));
            }
        }

        private async Task MontarEntidadesComplementares(UsuarioDTO usuarioDTO, Usuario usuario)
        {
            var cargo = await _cargoRepository.ObterCargoPorCodigoAsync(usuarioDTO.CargoCodigo);
            var departamento = await _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsync(usuarioDTO.DepartamentoCodigo);

            if (departamento is not null && cargo is not null)
            {
                usuario.CargoId = cargo.Id;
                usuario.CargoCodigo = cargo.Codigo;

                usuario.DepartamentoId = departamento.Id;
                usuario.DepartamentoCodigo = departamento.Codigo;
            }
            else
            {
                _messageModel.AddError("Cargo ou Departamento não encontrados. Cadastre-os ou tente novamente.");
                return;
            }
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
                CargoCodigo = usuario.CargoCodigo,
                DepartamentoCodigo = usuario.DepartamentoCodigo,
                Cargo = new CargoDTO() { Codigo = usuario.Cargo.Codigo, Descricao = usuario.Cargo.Descricao },
                Departamento = new DepartamentoDTO() { Codigo = usuario.Departamento.Codigo, Descricao = usuario.Departamento.Descricao },
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