﻿using Atron.Application.DTO;
using Atron.Application.Interfaces;
using Atron.Domain.ApiEntities;
using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Atron.Domain.Interfaces.ApplicationInterfaces;
using Atron.Domain.Interfaces.UsuarioInterfaces;
using Shared.Extensions;
using Shared.Interfaces.Mapper;
using Shared.Interfaces.Validations;
using Shared.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        // Sempre usar o repository pra acessar a camada de dados
        private readonly IApplicationMapService<UsuarioDTO, Usuario> _map;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUsuarioCargoDepartamentoRepository _usuarioCargoDepartamentoRepository;
        private readonly IRegisterApplicationRepository _registerApplicationRepository;
        private readonly IDepartamentoRepository _departamentoRepository;
        private readonly ICargoRepository _cargoRepository;
        private readonly ITarefaRepository _tarefaRepository;
        private readonly ISalarioRepository _salarioRepository;
        private readonly IValidateModel<Usuario> _validateModel;
        private readonly MessageModel _messageModel;

        public bool Registrar { get; set; }

        public UsuarioService(IApplicationMapService<UsuarioDTO, Usuario> map,
                              IUsuarioRepository repository,
                              IUsuarioCargoDepartamentoRepository usuarioCargoDepartamentoRepository,
                              IRegisterApplicationRepository registerApplicationRepository,
                              IDepartamentoRepository departamentoRepository,
                              ICargoRepository cargoRepository,
                              IValidateModel<Usuario> validateModel,
                              MessageModel messageModel,
                              ITarefaRepository tarefaRepository,
                              ISalarioRepository salarioRepository)
        {
            _map = map;
            _usuarioRepository = repository;
            _usuarioCargoDepartamentoRepository = usuarioCargoDepartamentoRepository;
            _registerApplicationRepository = registerApplicationRepository;
            _departamentoRepository = departamentoRepository;
            _cargoRepository = cargoRepository;
            _validateModel = validateModel;
            _messageModel = messageModel;
            _tarefaRepository = tarefaRepository;
            _salarioRepository = salarioRepository;
        }

        public async Task AtualizarAsync(string codigo, UsuarioDTO usuarioDTO)
        {
            if (usuarioDTO is null)
            {
                _messageModel.AddRegisterInvalidMessage(nameof(Usuario));
                return;
            }

            var entidade = _map.MapToEntity(usuarioDTO);

            _validateModel.Validate(entidade);

            if (!_messageModel.Messages.HasErrors())
            {
                if (!usuarioDTO.DepartamentoCodigo.IsNullOrEmpty())
                {
                    var departamentoBd = await _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsyncAsNoTracking(usuarioDTO.DepartamentoCodigo);
                    var cargoBd = await _cargoRepository.ObterCargoPorCodigoAsyncAsNoTracking(usuarioDTO.CargoCodigo);

                    entidade.UsuarioCargoDepartamentos = new List<UsuarioCargoDepartamento>()
                    {
                      new()
                      {
                         CargoId = cargoBd.Id,
                         CargoCodigo = cargoBd.Codigo,
                         DepartamentoId = departamentoBd.Id,
                         DepartamentoCodigo = departamentoBd.Codigo
                      }
                    };
                }

                var resultRepo = await _usuarioRepository.AtualizarUsuarioAsync(codigo, entidade);

                if (resultRepo)
                {
                    var register = new ApiRegister(entidade.Codigo, entidade.Email, usuarioDTO.Senha, usuarioDTO.Senha);

                    var registerResult = await _registerApplicationRepository.UpdateUserAccountAsync(register);

                    if (registerResult)
                    {
                        _messageModel.AddUpdateMessage(nameof(Usuario));
                    }
                }
            }
        }

        public async Task<UsuarioDTO> CriarAsync(UsuarioDTO usuarioDTO)
        {
            if (usuarioDTO is null)
            {
                _messageModel.AddRegisterInvalidMessage(nameof(Usuario));
                return usuarioDTO;
            }

            var entidade = _map.MapToEntity(usuarioDTO);

            if (!usuarioDTO.DepartamentoCodigo.IsNullOrEmpty())
            {
                var departamentoBd = await _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsyncAsNoTracking(usuarioDTO.DepartamentoCodigo);
                var cargoBd = await _cargoRepository.ObterCargoPorCodigoAsyncAsNoTracking(usuarioDTO.CargoCodigo);

                // A associação de cargo e departamento ao usuário será gravada
                // automaticamente informando o código do cargo e do departamento
                entidade.UsuarioCargoDepartamentos = new List<UsuarioCargoDepartamento>()
                {
                    new()
                    {
                        CargoId = cargoBd.Id,
                        CargoCodigo = cargoBd.Codigo,
                        DepartamentoId = departamentoBd.Id,
                        DepartamentoCodigo = departamentoBd.Codigo
                    }
                };
            }

            _validateModel.Validate(entidade);
            if (!_messageModel.Messages.HasErrors())
            {
                var usr = await _usuarioRepository.CriarUsuarioAsync(entidade);

                if (usr)
                {
                    var register = new ApiRegister()
                    {
                        UserName = entidade.Codigo,
                        Email = entidade.Email,
                        Password = usuarioDTO.Senha,
                        ConfirmPassword = usuarioDTO.Senha
                    };

                    var registerOk = await _registerApplicationRepository.RegisterUserAccountAsync(register);

                    if (registerOk)
                    {
                        _messageModel.AddSuccessMessage(nameof(Usuario));
                        _messageModel.AddMessage($"Usuário de acesso da aplicação: {usuarioDTO.Nome} cadastrado com sucesso");
                    }
                }
            }

            return usuarioDTO;
        }

        public async Task<UsuarioDTO> ObterPorCodigoAsync(string codigo)
        {
            var usuario = await _usuarioRepository.ObterUsuarioPorCodigoAsync(codigo);

            return usuario is null ? null : _map.MapToDTO(usuario);
        }

        public async Task<List<UsuarioDTO>> ObterTodosAsync()
        {
            var usuarios = await _usuarioRepository.ObterUsuariosAsync();
            return _map.MapToListDTO(usuarios.ToList());
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
                // Obtém todas as tarefas do usuário
                IEnumerable<Tarefa> tarefasDoUsuario = await _tarefaRepository.ObterTodasTarefasPorUsuario(usuario.Id, usuario.Codigo);

                if (tarefasDoUsuario.Any())
                {
                    foreach (var item in tarefasDoUsuario)
                    {
                        await _tarefaRepository.RemoverRepositoryAsync(item);
                    }
                }

                // Salário sempre será um registro por usuário
                Salario salarioDoUsuario = await _salarioRepository.ObterSalarioPorUsuario(usuario.Id, usuario.Codigo);

                if (salarioDoUsuario is not null)
                {
                    await _salarioRepository.RemoverRepositoryAsync(salarioDoUsuario);
                }
      
                // O relacionamento será apenas um por usuário
                var associacao = await _usuarioCargoDepartamentoRepository.ObterPorChaveDoUsuario(usuario.Id, usuario.Codigo);

                if (associacao is not null)
                {
                    await _usuarioCargoDepartamentoRepository.RemoverRepositoryAsync(associacao);
                }

                // Como o usuário já existe será removido do cadastro da aplicação
                await _registerApplicationRepository.DeleteAccountUserAsync(codigo);

                // Remove o usuário por completo
                await _usuarioRepository.RemoverUsuarioAsync(usuario);

                _messageModel.AddRegisterRemovedSuccessMessage(nameof(Usuario));
            }
        }
    }
}