using Application.DTO.ApiDTO;
using Application.Interfaces.ApplicationInterfaces;
using Application.Services.AuthServices.Bases;
using Application.Specifications.UsuarioSpecifications;
using Domain.ApiEntities;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Identity;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.DTOS.Requests;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System;
using System.Threading.Tasks;
using System.Web;

namespace Application.Services.AuthServices
{
    public class RegistroUsuarioService : ServiceBase, IRegistroUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPerfilDeAcessoRepository _perfilDeAcessoRepository;
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
            _perfilDeAcessoRepository = perfilDeAcessoRepository;
        }

        public async Task<Resultado> RegistrarUsuario(UsuarioRegistroDTO usuarioRegistroDTO)
        {
            var _usuarioIdentityRepository = ObterService<IUsuarioIdentityRepository>();
            var _emailService = ObterService<IEmailService>();

            var usuarioRegistro = new UsuarioRegistro
            {
                CodigoDeAcesso = usuarioRegistroDTO.Codigo.ToUpper(),
                Email = usuarioRegistroDTO.Email,
                Senha = usuarioRegistroDTO.Senha,
                ConfirmarSenha = usuarioRegistroDTO.ConfirmaSenha
            };

            GetValidator<UsuarioRegistro>().Validate(usuarioRegistro);

            if (_messageModel.Notificacoes.HasErrors()) return;

            var contaExiste = await _usuarioIdentityRepository
                .ContaExisteRepositoryAsync(usuarioRegistro.CodigoDeAcesso, usuarioRegistro.Email);

            if (contaExiste)
            {
                _messageModel.AdicionarErro("Usuário já cadastrado.");
                return;
            }

            var registrado = await _usuarioIdentityRepository
                .RegistrarContaDeUsuarioRepositoryAsync(
                    usuarioRegistro.CodigoDeAcesso,
                    usuarioRegistro.Email,
                    usuarioRegistro.Senha);

            if (!registrado) return;

            var usuario = new Usuario
            {
                Codigo = usuarioRegistroDTO.Codigo.ToUpper(),
                Nome = usuarioRegistroDTO.Nome,
                Sobrenome = usuarioRegistroDTO.Sobrenome,
                DataNascimento = usuarioRegistroDTO.DataNascimento?.ToDateTime(TimeOnly.MinValue),
                Email = usuarioRegistroDTO.Email
            };

            //ObterValidador<Usuario>().Validate(usuario);

            var usuarioSpec = new UsuarioSpecification(usuario.Codigo, usuario.Email);
            if (!usuarioSpec.IsSatisfiedBy(usuario))
            {
                usuarioSpec.Errors.ForEach(_messageModel.AdicionarErro);
                return;
            }

            if (_messageModel.Notificacoes.HasErrors()) return;

            var usuarioGravado = await _usuarioRepository.CriarUsuarioAsync(usuario);
            if (!usuarioGravado) return;

            var usuarioBd = await _usuarioRepository.ObterUsuarioPorCodigoAsync(usuario.Codigo);

            var perfilDeAcesso = await _perfilDeAcessoRepository
                .ObterPerfilPorCodigoRepositoryAsync(usuarioRegistroDTO.CodigoPerfilDeAcesso);

            if (perfilDeAcesso != null)
            {
                await _perfilDeAcessoUsuarioRepository.CriarPerfilRepositoryAsync(new PerfilDeAcessoUsuario
                {
                    PerfilDeAcessoId = perfilDeAcesso.Id,
                    PerfilDeAcessoCodigo = perfilDeAcesso.Codigo,
                    UsuarioId = usuarioBd.Id,
                    UsuarioCodigo = usuarioBd.Codigo
                });
            }

            var token = await _usuarioIdentityRepository
                .GerarTokenConfirmacaoEmailAsync(usuarioRegistro.CodigoDeAcesso);

            string baseUri = !string.IsNullOrEmpty(usuarioRegistroDTO.ClientUri)
                ? usuarioRegistroDTO.ClientUri
                : "http://localhost:4200";

            string link = $"{baseUri}/confirmar-email?usuarioCodigo={usuarioRegistro.CodigoDeAcesso}&token={HttpUtility.UrlEncode(token)}";

            try
            {
                await _emailService.EnviarAsync(new EmailRequest
                {
                    Assunto = "Confirme seu cadastro - AtronTracker",
                    Mensagem = CorpoDoEmailDeCadastro(usuario, link),
                    EmailsDestino = [usuarioRegistro.Email]
                });
            }
            catch { }

            _messageModel.AdicionarMensagem(
                $"Usuário {usuario.Nome} {usuario.Sobrenome}: cadastro realizado com sucesso! Verifique seu e-mail para confirmar.");
        }

        public async Task<bool> ConfirmarEmail(string codigoUsuario, string token)
        {
            var _usuarioIdentityRepository = ObterService<IUsuarioIdentityRepository>();
            var resultado = await _usuarioIdentityRepository.ConfirmarEmailAsync(codigoUsuario, token);

            if (!resultado)
                _messageModel.AdicionarErro("Falha ao confirmar e-mail. Token inválido ou expirado.");
            else
                _messageModel.AdicionarMensagem("E-mail confirmado com sucesso!");

            return resultado;
        }

        private static string CorpoDoEmailDeCadastro(Usuario usuario, string link)
        {
            return $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e0e0e0; border-radius: 10px;'>
                    <h1 style='color: #2c3e50;'>Bem-vindo(a) ao Atron!</h1>
                    <p>Olá, <strong>{usuario.Nome}</strong>!</p>
                    <p>Seu cadastro foi recebido. Para confirmar seu e-mail, clique no botão abaixo:</p>
                    <div style='text-align: center; margin: 30px 0;'>
                        <a href='{link}' style='background-color: #007bff; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; font-weight: bold;'>Confirmar meu E-mail</a>
                    </div>
                    <p style='font-size: 12px; color: #999; word-break: break-all;'>{link}</p>
                    <p style='font-size: 12px; color: #aaa;'>Se você não criou esta conta, ignore este e-mail.</p>
                </div>";
        }
    }
}