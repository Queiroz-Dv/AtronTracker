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

            await MontarEntidadesComplementares(usuarioDTO);

            var entidade = _mapper.Map<Usuario>(usuarioDTO);

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

            await MontarEntidadesComplementares(usuarioDTO);

            var usuario = _mapper.Map<Usuario>(usuarioDTO);

            _messageModel.Validate(usuario);

            if (!_messageModel.Messages.HasErrors())
            {
                await _usuarioRepository.CriarUsuarioAsync(usuario);
                _messageModel.AddSuccessMessage(nameof(Usuario));
            }
        }

        private async Task MontarEntidadesComplementares(UsuarioDTO usuarioDTO)
        {
            var cargo = await _cargoRepository.ObterCargoPorCodigoAsync(usuarioDTO.CargoCodigo);
            var departamento = await _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsync(usuarioDTO.DepartamentoCodigo);

            if (departamento is not null && cargo is not null)
            {                
                usuarioDTO.CargoId = cargo.Id;
                usuarioDTO.CargoCodigo = cargo.Codigo;

                usuarioDTO.DepartamentoId = departamento.Id;
                usuarioDTO.DepartamentoCodigo = departamento.Codigo;                
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

            var usuarioDTO = _mapper.Map<UsuarioDTO>(usuario);

            var usuarioPreenchido = new UsuarioDTO()
            {
                Id = usuarioDTO.Id,
                Codigo = usuarioDTO.Codigo,
                Nome = usuarioDTO.Nome,
                Sobrenome = usuarioDTO.Sobrenome,
                Salario = usuarioDTO.Salario,
                DataNascimento = usuarioDTO.DataNascimento,
                CargoCodigo = usuarioDTO.CargoCodigo,
                DepartamentoCodigo = usuarioDTO.DepartamentoCodigo,
                Cargo = new CargoDTO() { Codigo = usuarioDTO.Cargo.Codigo, Descricao = usuarioDTO.Cargo.Descricao },
                Departamento = new DepartamentoDTO() { Codigo = usuarioDTO.Departamento.Codigo, Descricao = usuarioDTO.Departamento.Descricao },
            };

            return usuarioPreenchido;
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