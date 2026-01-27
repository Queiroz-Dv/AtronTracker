using Application.DTO.ApiDTO;
using Application.Interfaces.ApplicationInterfaces;
using Application.Services.AuthServices.Bases;
using Domain.ApiEntities;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Identity;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Extensions;
using System;
using System.Threading.Tasks;
using Application.Specifications.UsuarioSpecifications;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using System.Web;

namespace Application.Services.AuthServices
{
    public class RegistroUsuarioService : ServiceBase, IRegistroUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPerfilDeAcessoRepository perfilDeAcessoRepository;
        private readonly IPerfilDeAcessoUsuarioRepository _perfilDeAcessoUsuarioRepository;
        private readonly Notifiable _messageModel;

        public RegistroUsuarioService(
            IAccessorService accessor,
            IUsuarioRepository usuarioRepository,
            Notifiable messageModel,
            IPerfilDeAcessoUsuarioRepository perfilDeAcessoUsuarioRepository,
            IPerfilDeAcessoRepository perfilDeAcessoRepository) : base(accessor)
        {
            _usuarioRepository = usuarioRepository;
            _messageModel = messageModel;
            _perfilDeAcessoUsuarioRepository = perfilDeAcessoUsuarioRepository;
            this.perfilDeAcessoRepository = perfilDeAcessoRepository;
        }

        public async Task RegistrarUsuario(UsuarioRegistroDTO usuarioRegistroDTO)
        {
            var _usuarioIdentityRepository = ObterService<IUsuarioIdentityRepository>();
            var _emailService = ObterService<IEmailService>();

            var usuarioRegistro = new UsuarioRegistro()
            {
                CodigoDeAcesso = usuarioRegistroDTO.Codigo.ToUpper(),
                Email = usuarioRegistroDTO.Email,
                Senha = usuarioRegistroDTO.Senha,
                ConfirmarSenha = usuarioRegistroDTO.ConfirmaSenha
            };

            GetValidator<UsuarioRegistro>().Validate(usuarioRegistro);

            if (!Messages.Notificacoes.HasErrors())
            {
                var contaExiste = await _usuarioIdentityRepository.ContaExisteRepositoryAsync(usuarioRegistro.CodigoDeAcesso, usuarioRegistro.Email);

                if (contaExiste)
                {
                    _messageModel.AdicionarErro("Usuário já cadastrado.");
                    return;
                }

                var registrado = await _usuarioIdentityRepository.RegistrarContaDeUsuarioRepositoryAsync(usuarioRegistro.CodigoDeAcesso, usuarioRegistro.Email, usuarioRegistro.Senha);

                if (registrado)
                {
                    var usuario = new Usuario()
                    {
                        Codigo = usuarioRegistroDTO.Codigo.ToUpper(),
                        Nome = usuarioRegistroDTO.Nome,
                        Sobrenome = usuarioRegistroDTO.Sobrenome,
                        DataNascimento = usuarioRegistroDTO.DataNascimento?.ToDateTime(TimeOnly.MinValue),
                        Email = usuarioRegistroDTO.Email
                    };

                    GetValidator<Usuario>().Validate(usuario);

                    var usuarioSpec = new UsuarioSpecification(usuario.Codigo, usuario.Email);
                    if (!usuarioSpec.IsSatisfiedBy(usuario))
                    {
                        usuarioSpec.Errors.ForEach(_messageModel.AdicionarErro);
                    }

                    if (!_messageModel.Notificacoes.HasErrors())
                    {
                        var usuarioGravado = await _usuarioRepository.CriarUsuarioAsync(usuario);
                        if (usuarioGravado)
                        {
                            var perfilDeAcesso = await perfilDeAcessoRepository.ObterPerfilPorCodigoRepositoryAsync(usuarioRegistroDTO.CodigoPerfilDeAcesso);
                            var usuarioBd = await _usuarioRepository.ObterUsuarioPorCodigoAsync(usuario.Codigo);

                            if (perfilDeAcesso != null)
                            {
                                var perfilDeAcessoUsuario = new PerfilDeAcessoUsuario()
                                {
                                    PerfilDeAcessoId = perfilDeAcesso.Id,
                                    PerfilDeAcessoCodigo = perfilDeAcesso.Codigo,
                                    UsuarioId = usuarioBd.Id,
                                    UsuarioCodigo = usuarioBd.Codigo
                                };

                                await _perfilDeAcessoUsuarioRepository.CriarPerfilRepositoryAsync(perfilDeAcessoUsuario);
                            }
                            
                            var token = await _usuarioIdentityRepository.GerarTokenConfirmacaoEmailAsync(usuarioRegistro.CodigoDeAcesso);
                            string baseUri = ObterUri(usuarioRegistroDTO);
                            string link = MontarUrl(usuarioRegistro, token, baseUri);
                            var corpo = CorpoDoEmailDeCadastro(usuario, link);

                            var emailRequest = new Shared.Application.DTOS.Requests.EmailRequest
                            {
                                Assunto = "Confirme seu cadastro - AtronTracker",
                                Mensagem = corpo,
                                EmailsDestino = [usuarioRegistro.Email]
                            };

                            var emailEnviado = await _emailService.EnviarAsync(emailRequest);

                            if (!emailEnviado.TeveSucesso)
                            {
                                // Rollback
                                if (perfilDeAcesso != null)
                                {
                                    var perfilUsuarioParaDeletar = await _perfilDeAcessoUsuarioRepository
                                        .ObterPerfilDeAcessoPorCodigoRepositoryAsync(perfilDeAcesso.Codigo);
                                    if (perfilUsuarioParaDeletar != null)
                                    {
                                        await _perfilDeAcessoUsuarioRepository.DeletarRelacionamento(perfilUsuarioParaDeletar);
                                    }
                                }

                                var usuarioParaDeletar = await _usuarioRepository.ObterPorCodigoRepositoryAsync(usuarioBd.Codigo);
                                if (usuarioParaDeletar != null)
                                {
                                    await _usuarioRepository.RemoverUsuarioAsync(usuarioParaDeletar);
                                }

                                await _usuarioIdentityRepository.DeletarContaUserRepositoryAsync(usuarioRegistro.CodigoDeAcesso);

                                _messageModel.AdicionarErro("Falha ao enviar e-mail de confirmação. O cadastro foi revertido. Tente novamente.");
                                return;
                            }

                            _messageModel.AdicionarMensagem($"Usuário {usuario.Nome} {usuario.Sobrenome}: cadastro realizado com sucesso! Verifique seu e-mail para confirmar.");
                        }
                    }
                }
            }

            return;
        }

        private static string ObterUri(UsuarioRegistroDTO usuarioRegistroDTO)
        {
            return !string.IsNullOrEmpty(usuarioRegistroDTO.ClientUri)
             ? usuarioRegistroDTO.ClientUri
             : "http://localhost:4200";
        }

        private static string MontarUrl(UsuarioRegistro usuarioRegistro, string token, string baseUri)
        {
            return $"{baseUri}/confirmar-email?usuarioCodigo={usuarioRegistro.CodigoDeAcesso}&token={HttpUtility.UrlEncode(token)}";
        }

        private static string CorpoDoEmailDeCadastro(Usuario usuario, string link)
        {
            return $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e0e0e0; border-radius: 10px;'>
                    <div style='text-align: center; margin-bottom: 20px;'>
                        <h1 style='color: #2c3e50;'>Bem-vindo(a) ao Atron!</h1>
                    </div>
                    <p style='font-size: 16px; color: #555;'>Olá, <strong>{usuario.Nome}</strong>!</p>
                    <p style='font-size: 16px; color: #555;'>Seu cadastro foi recebido com sucesso. Para garantir a segurança da sua conta e acessar o sistema, por favor, confirme seu e-mail clicando no botão abaixo:</p>
                    
                    <div style='text-align: center; margin: 30px 0;'>
                        <a href='{link}' style='background-color: #007bff; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; font-weight: bold; font-size: 16px;'>Confirmar meu E-mail</a>
                    </div>

                    <p style='font-size: 14px; color: #777;'>Se o botão não funcionar, copie e cole o link abaixo no seu navegador:</p>
                    <p style='font-size: 12px; color: #999; word-break: break-all;'>{link}</p>
                    
                    <hr style='border: 0; border-top: 1px solid #eee; margin: 20px 0;'>
                    <p style='font-size: 12px; color: #aaa; text-align: center;'>Se você não criou esta conta, por favor, ignore este e-mail.</p>
                </div>";
        }

        public async Task<bool> ConfirmarEmail(string codigoUsuario, string token)
        {
            var _usuarioIdentityRepository = ObterService<IUsuarioIdentityRepository>();
            var resultado = await _usuarioIdentityRepository.ConfirmarEmailAsync(codigoUsuario, token);
            
            if (!resultado)
            {
                _messageModel.AdicionarErro("Falha ao confirmar e-mail. Token inválido ou expirado.");
            }

            _messageModel.AdicionarMensagem("E-mail confirmado com sucesso!"); 

            return resultado;
        }
    }
}