using Application.DTO;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Identity;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTO.ApiDTO;
using Shared.Application.Interfaces.Service;
using Shared.Application.DTOS.Email;
using Shared.Domain.ValueObjects;
using Shared.Application.DTOS.Requests;

namespace Application.Services.EntitiesServices
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
        private readonly IValidateModelService<Usuario> _validateModel;
        private readonly IEmailService _emailService;
        private readonly Notifiable _messageModel;

        public UsuarioService(IAsyncApplicationMapService<UsuarioDTO, UsuarioIdentity> map,
                              IUsuarioRepository repository,
                              IUsuarioCargoDepartamentoRepository usuarioCargoDepartamentoRepository,
                              IDepartamentoRepository departamentoRepository,
                              ICargoRepository cargoRepository,
                              ITarefaRepository tarefaRepository,
                              ISalarioRepository salarioRepository,
                              IValidateModelService<Usuario> validateModel,
                              Notifiable messageModel,
                              IUsuarioIdentityRepository usuarioIdentityRepository,
                              IEmailService emailService)
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
            _emailService = emailService;
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

            // Validação assíncrona de e-mail único
            if (!string.IsNullOrEmpty(entidade.Email))
            {
                var emailExiste = await _usuarioRepository.VerificarEmailExistenteAsync(entidade.Email);
                if (emailExiste)
                {
                    _messageModel.AdicionarErro("O e-mail informado já está cadastrado no sistema.");
                    return null;
                }
            }

            if (!_messageModel.Notificacoes.HasErrors())
            {
                var usuarioExiste = await _usuarioRepository.ObterUsuarioPorCodigoAsync(entidade.Codigo);
                if (usuarioExiste != null)
                {
                    _messageModel.AdicionarErro("Código de usuário informado já existe. Tente outro novamente.");
                    return null;
                }

                await _usuarioRepository.CriarUsuarioAsync(entidade);
                var usuarioRegistrado = await _usuarioRepository.ObterUsuarioPorCodigoAsync(entidade.Codigo);
                if (usuarioRegistrado != null)
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
                        // Enviar e-mail de boas-vindas
                        try
                        {
                            var emailMessage = CriarEmailBoasVindas(entidade.Email, entidade.Nome);
                            await _emailService.EnviarAsync(emailMessage);
                        }
                        catch
                        {
                            // Log ou tratamento de erro - não interrompe o fluxo de criação
                        }

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
            return await _map.MapToListDTOAsync(usuarios);
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

        /// <summary>
        /// Cria a mensagem de e-mail de boas-vindas para novo usuário.
        /// </summary>
        private static EmailRequest CriarEmailBoasVindas(string destinatario, string nomeUsuario)
        {
            var assunto = "Bem-vindo ao Sistema Atron!";
            var corpo = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f4f4f4; }}
        .container {{ max-width: 600px; margin: 0 auto; background-color: #ffffff; padding: 30px; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }}
        .header {{ text-align: center; padding-bottom: 20px; border-bottom: 2px solid #007bff; }}
        .header h1 {{ color: #007bff; margin: 0; }}
        .content {{ padding: 20px 0; }}
        .content p {{ color: #333; line-height: 1.6; }}
        .footer {{ text-align: center; padding-top: 20px; border-top: 1px solid #eee; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🎉 Bem-vindo ao Sistema Atron!</h1>
        </div>
        <div class='content'>
            <p>Olá, <strong>{nomeUsuario}</strong>!</p>
            <p>Sua conta foi criada com sucesso no Sistema Atron.</p>
            <p>Agora você pode acessar o sistema utilizando suas credenciais de login.</p>
            <p>Se você tiver alguma dúvida, entre em contato com o suporte.</p>
        </div>
        <div class='footer'>
            <p>Este é um e-mail automático. Por favor, não responda.</p>
            <p>&copy; {System.DateTime.Now.Year} Sistema Atron. Todos os direitos reservados.</p>
        </div>
    </div>
</body>
</html>";

            return new EmailRequest
            {
                EmailsDestino = new System.Collections.Generic.List<string> { destinatario },
                Assunto = assunto,
                Mensagem = corpo
            };
        }
    }
}