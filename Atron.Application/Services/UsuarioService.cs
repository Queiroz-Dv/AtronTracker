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

            var usuario = _mapper.Map<Usuario>(usuarioDTO);

            var cargoExiste = _cargoRepository.CargoExiste(usuario.CargoCodigo);
            var departamentoExiste = _departamentoRepository.DepartamentoExiste(usuario.DepartamentoCodigo);
            var usuarioExiste = _usuarioRepository.UsuarioExiste(usuarioDTO.Codigo);

            if (departamentoExiste && cargoExiste && usuarioExiste)
            {
                var identificadorUsuario = await _usuarioRepository.ObterUsuarioPorCodigoAsync(usuario.Codigo);
                var cargo = await _cargoRepository.ObterCargoPorCodigoAsync(usuario.CargoCodigo);
                var departamento = await _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsync(usuario.DepartamentoCodigo);
                usuario.SetId(identificadorUsuario.Id);
                usuario.CargoId = cargo.Id;
                usuario.DepartamentoId = departamento.Id;
            }           

            _messageModel.Validate(usuario);

            if (!_messageModel.Messages.HasErrors())
            {
                await _usuarioRepository.AtualizarUsuarioAsync(usuario);
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

            var cargoExiste = _cargoRepository.CargoExiste(usuarioDTO.CargoCodigo);
            var departamentoExiste = _departamentoRepository.DepartamentoExiste(usuarioDTO.DepartamentoCodigo);

            if (departamentoExiste && cargoExiste)
            {
                var cargo = await _cargoRepository.ObterCargoPorCodigoAsync(usuarioDTO.CargoCodigo);
                var departamento = await _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsync(usuarioDTO.DepartamentoCodigo);

                usuarioDTO.CargoId = cargo.Id;
                usuarioDTO.DepartamentoId = departamento.Id;
            }
            else
            {
                _messageModel.AddError("Cargo ou Departamento não encontrados. Cadastre-os ou tente novamente.");
                return;
            }

            var usuario = _mapper.Map<Usuario>(usuarioDTO);

            _messageModel.Validate(usuario);

            if (!_messageModel.Messages.HasErrors())
            {
                await _usuarioRepository.CriarUsuarioAsync(usuario);
                _messageModel.AddSuccessMessage(nameof(Usuario));
            }
        }

        public async Task<UsuarioDTO> ObterPorCodigoAsync(string codigo)
        {
            var cargos = await _cargoRepository.ObterCargosAsync();
            var departamentos = await _departamentoRepository.ObterDepartmentosAsync();
            var usuario = await _usuarioRepository.ObterUsuarioPorCodigoAsync(codigo);

            var cargosDTOs = _mapper.Map<IEnumerable<CargoDTO>>(cargos);
            var departamentosDTOs = _mapper.Map<IEnumerable<DepartamentoDTO>>(departamentos);
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
                Cargo = new CargoDTO() { Codigo = usuarioDTO.CargoCodigo, Descricao = cargosDTOs.FirstOrDefault(crg => crg.Id == usuarioDTO.CargoId).Descricao, DepartamentoCodigo = usuarioDTO.DepartamentoCodigo },
                Departamento = new DepartamentoDTO() { Codigo = usuarioDTO.DepartamentoCodigo, Descricao = departamentosDTOs.FirstOrDefault(dpt => dpt.Id == usuarioDTO.DepartamentoId).Descricao },
            };

            return usuarioPreenchido;
        }

        public async Task<List<UsuarioDTO>> ObterTodosAsync()
        {
            var cargos = await _cargoRepository.ObterCargosAsync();
            var departamentos = await _departamentoRepository.ObterDepartmentosAsync();
            var usuarios = await _usuarioRepository.ObterUsuariosAsync();

            var cargosDTOs = _mapper.Map<IEnumerable<CargoDTO>>(cargos);
            var departamentosDTOs = _mapper.Map<IEnumerable<DepartamentoDTO>>(departamentos);
            var usuariosDTOs = _mapper.Map<IEnumerable<UsuarioDTO>>(usuarios);

            var usuarioPreenchido = (from usr in usuariosDTOs
                                     join dpt in departamentosDTOs on usr.DepartamentoId equals dpt.Id
                                     join crg in cargosDTOs on usr.CargoId equals crg.Id
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
                                             Codigo = usr.CargoCodigo,
                                             Descricao = crg.Descricao,
                                             DepartamentoCodigo = usr.DepartamentoCodigo
                                         },
                                         Departamento = new DepartamentoDTO()
                                         {
                                             Codigo = usr.DepartamentoCodigo,
                                             Descricao = dpt.Descricao
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