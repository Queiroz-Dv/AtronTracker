using Atron.Application.DTO;
using Atron.Application.DTO.ApiDTO;
using Atron.Application.Interfaces.Services;
using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Atron.Domain.Interfaces.Identity;
using Atron.Domain.Interfaces.UsuarioInterfaces;
using Shared.Extensions;
using Shared.Interfaces.Mapper;
using Shared.Interfaces.Validations;
using Shared.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.Application.Services.EntitiesServices
{
    public class UsuarioService : IUsuarioService
    {
        // Sempre usar o repository pra acessar a camada de dados
        private readonly IAsyncApplicationMapService<UsuarioDTO, UsuarioIdentity> _map;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUsuarioCargoDepartamentoRepository _usuarioCargoDepartamentoRepository;
        private readonly IDepartamentoRepository _departamentoRepository;
        private readonly ICargoRepository _cargoRepository;
        private readonly ITarefaRepository _tarefaRepository;
        private readonly ISalarioRepository _salarioRepository;
        private readonly IUsuarioIdentityRepository _usuarioIdentityRepository;
        private readonly IValidateModel<Usuario> _validateModel;
        private readonly MessageModel _messageModel;

        public UsuarioService(IAsyncApplicationMapService<UsuarioDTO, UsuarioIdentity> map,
                              IUsuarioRepository repository,
                              IUsuarioCargoDepartamentoRepository usuarioCargoDepartamentoRepository,
                              IDepartamentoRepository departamentoRepository,
                              ICargoRepository cargoRepository,
                              ITarefaRepository tarefaRepository,
                              ISalarioRepository salarioRepository,
                              IValidateModel<Usuario> validateModel,
                              MessageModel messageModel,
                              IUsuarioIdentityRepository usuarioIdentityRepository)
        {
            _map = map;
            _usuarioRepository = repository;
            _usuarioCargoDepartamentoRepository = usuarioCargoDepartamentoRepository;
            _departamentoRepository = departamentoRepository;
            _cargoRepository = cargoRepository;
            _validateModel = validateModel;
            _messageModel = messageModel;
            _tarefaRepository = tarefaRepository;
            _salarioRepository = salarioRepository;
            _usuarioIdentityRepository = usuarioIdentityRepository;
        }

        public async Task AtualizarAsync(string codigo, UsuarioDTO usuarioDTO)
        {
            if (usuarioDTO is null)
            {
                _messageModel.MensagemRegistroInvalido(nameof(Usuario));
                return;
            }

            var usuarioIdentity = await _map.MapToEntityAsync(usuarioDTO);

            var entidade = new Usuario()
            {
                Codigo = usuarioDTO.Codigo,
                Nome = usuarioDTO.Nome,
                Sobrenome = usuarioDTO.Sobrenome,
                DataNascimento = usuarioDTO.DataNascimento,
                Email = usuarioDTO.Email,
                SalarioAtual = usuarioDTO.Salario,
            };

            _validateModel.Validate(entidade);

            if (!_messageModel.Notificacoes.HasErrors())
            {
                var atualizado = await _usuarioRepository.AtualizarUsuarioAsync(codigo, entidade);

                if (atualizado)
                {
                    var usuarioRegistroAtualizado = await _usuarioIdentityRepository.AtualizarUserIdentityRepositoryAsync(entidade.Codigo, entidade.Email, usuarioDTO.Senha);

                    var usuario = await _usuarioRepository.ObterUsuarioPorCodigoAsync(codigo);
                    var departamentoBd = await _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsyncAsNoTracking(usuarioDTO.DepartamentoCodigo);
                    var cargoBd = await _cargoRepository.ObterCargoPorCodigoAsync(usuarioDTO.CargoCodigo);

                    var relacionamento = new UsuarioCargoDepartamento()
                    {
                        UsuarioId = entidade.Id,
                        UsuarioCodigo = usuarioDTO.Codigo,
                        CargoId = cargoBd.Id,
                        CargoCodigo = cargoBd.Codigo,
                        DepartamentoId = departamentoBd.Id,
                        DepartamentoCodigo = departamentoBd.Codigo
                    };

                    var relacionamentoAtualizado = await _usuarioCargoDepartamentoRepository.AtualizarRepositoryAsync(relacionamento);

                    if (usuarioRegistroAtualizado && relacionamentoAtualizado)
                    {
                        _messageModel.AdicionarMensagem($"Informações do usuário: {usuarioDTO.Nome} atualizadas com sucesso.");
                    }
                }
            }
        }

        public async Task<UsuarioDTO> CriarAsync(UsuarioDTO usuarioDTO)
        {
            if (usuarioDTO is null)
            {
                _messageModel.MensagemRegistroInvalido(nameof(Usuario));
                return usuarioDTO;
            }

            var usuarioIdentity = await _map.MapToEntityAsync(usuarioDTO);

            var entidade = new Usuario()
            {
                Codigo = usuarioDTO.Codigo,
                Nome = usuarioDTO.Nome,
                Sobrenome = usuarioDTO.Sobrenome,
                DataNascimento = usuarioDTO.DataNascimento,
                Email = usuarioDTO.Email,
                SalarioAtual = usuarioDTO.Salario
            };

            _validateModel.Validate(entidade);
            if (!_messageModel.Notificacoes.HasErrors())
            {
                var usuarioExiste = await _usuarioRepository.ObterUsuarioPorCodigoAsync(entidade.Codigo);
                if (usuarioExiste != null)
                {
                    _messageModel.AdicionarErro("Código de usuário informado já existe. Tente outro novamente.");
                    return null;
                }

                var usuarioCriado = await _usuarioRepository.CriarUsuarioAsync(entidade);

                if (usuarioCriado)
                {
                    var usuarioRegistro = new UsuarioRegistroDTO()
                    {
                        Codigo = entidade.Codigo,
                        CodigoPerfilDeAcesso = usuarioDTO.PerfilDeAcessoCodigo,
                        Email = entidade.Email,
                        Senha = usuarioDTO.Senha,
                        ConfirmaSenha = usuarioDTO.Senha
                    };

                    var contaRegistrada = await _usuarioIdentityRepository.RegistrarContaDeUsuarioRepositoryAsync(usuarioRegistro.Codigo,
                                                                                       usuarioRegistro.Email,
                                                                                       usuarioRegistro.Senha);


                    if (!usuarioDTO.DepartamentoCodigo.IsNullOrEmpty())
                    {
                        var departamentoBd = await _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsyncAsNoTracking(usuarioDTO.DepartamentoCodigo);
                        var cargoBd = await _cargoRepository.ObterCargoPorCodigoAsync(usuarioDTO.CargoCodigo);

                        await _usuarioCargoDepartamentoRepository.GravarAssociacaoUsuarioCargoDepartamento(entidade, cargoBd, departamentoBd);
                    }

                    if (!_messageModel.Notificacoes.HasErrors())
                    {
                        _messageModel.AdicionarMensagem($"Usuário de acesso da aplicação: {usuarioDTO.Nome} cadastrado com sucesso");
                    }
                }
            }

            return usuarioDTO;
        }

        public async Task<UsuarioDTO> ObterPorCodigoAsync(string codigo)
        {
            var usuario = await _usuarioRepository.ObterUsuarioPorCodigoAsync(codigo);

            return usuario is null ? null : await _map.MapToDTOAsync(usuario);
        }

        public async Task<List<UsuarioDTO>> ObterTodosAsync()
        {
            var usuarios = await _usuarioRepository.ObterTodosUsuariosDoIdentity();
            return await _map.MapToListDTOAsync(usuarios.OrderByDescending(c => c.Codigo).ToList());
        }

        public async Task RemoverAsync(string codigo)
        {
            var usuario = await _usuarioRepository.ObterUsuarioPorCodigoAsync(codigo);

            if (usuario == null)
            {
                _messageModel.MensagemRegistroNaoEncontrado(nameof(Usuario));
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
                // await _registerApplicationRepository.DeleteAccountUserAsync(codigo);

                // Remove o usuário por completo

                var entidade = new Usuario()
                {
                    Codigo = usuario.Codigo,
                    Nome = usuario.Nome,
                    Sobrenome = usuario.Sobrenome,
                    DataNascimento = usuario.DataNascimento,
                    Email = usuario.Email,
                    Salario = usuario.Salario,
                };

                await _usuarioRepository.RemoverUsuarioAsync(entidade);

                _messageModel.MensagemRegistroRemovido(nameof(Usuario));
            }
        }
    }
}