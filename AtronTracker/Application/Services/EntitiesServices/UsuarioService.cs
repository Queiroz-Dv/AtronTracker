using Application.DTO;
using Application.DTO.Request;
using System.Collections.Generic;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Identity;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.DTOS.Requests;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services.EntitiesServices
{
    /// <summary>
    /// Classe de serviço para usuários
    /// </summary>
    public class UsuarioService : IUsuarioService
    {
        private const string UsuarioContexto = nameof(Usuario);
        private readonly IValidador<UsuarioRequest> _validador;
        private readonly IAsyncMap<UsuarioRequest, Usuario> _mapService;
        private readonly IAsyncMap<UsuarioDTO, Usuario> _asyncMap;

        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUsuarioCargoDepartamentoRepository _usuarioCargoDepartamentoRepository;
        private readonly IDepartamentoRepository _departamentoRepository;
        private readonly ICargoRepository _cargoRepository;
        private readonly ITarefaRepository _tarefaRepository;
        private readonly ISalarioRepository _salarioRepository;
        private readonly IUsuarioIdentityRepository _usuarioIdentityRepository;
        private readonly IEmailService _emailService;

        public UsuarioService(
            IValidador<UsuarioRequest> validador,
            IAsyncMap<UsuarioRequest, Usuario> mapService,
            IAsyncMap<UsuarioDTO, Usuario> asyncMap,
            IUsuarioRepository repository,
            IUsuarioCargoDepartamentoRepository usuarioCargoDepartamentoRepository,
            IDepartamentoRepository departamentoRepository,
            ICargoRepository cargoRepository,
            ITarefaRepository tarefaRepository,
            ISalarioRepository salarioRepository,
            IUsuarioIdentityRepository usuarioIdentityRepository,
            IEmailService emailService)
        {
            _validador = validador;
            _asyncMap = asyncMap;
            _mapService = mapService;
            _usuarioRepository = repository;
            _usuarioCargoDepartamentoRepository = usuarioCargoDepartamentoRepository;
            _departamentoRepository = departamentoRepository;
            _cargoRepository = cargoRepository;
            _tarefaRepository = tarefaRepository;
            _salarioRepository = salarioRepository;
            _usuarioIdentityRepository = usuarioIdentityRepository;
            _emailService = emailService;
        }

        #region Criação

        public async Task<Resultado<UsuarioRequest>> CriarAsync(UsuarioRequest request)
        {
            var messages = _validador.Validar(request);
            if (messages.Any())
            {
                var resultado = Resultado<UsuarioRequest>.Falha(messages.FirstOrDefault()); // Falha agora aceita um erro ou notification
                foreach(var msg in messages.Skip(1)) resultado.Adicionar(msg);
                return resultado; 
            }

            var usuarioExistente = await _usuarioRepository.ObterUsuarioPorCodigoAsync(request.Codigo.ToUpper());
            if (usuarioExistente != null) return Resultado<UsuarioRequest>.Falha(UsuarioResource.ErroUsuarioExistente);

            if (!string.IsNullOrEmpty(request.Email))
            {
                var emailExiste = await _usuarioRepository.VerificarEmailExistenteAsync(request.Email);
                if (emailExiste) return Resultado<UsuarioRequest>.Falha(EmailResource.ErroEmailUtilizado);
            }

            var usuario = await _mapService.MapToEntityAsync(request);
            var usuarioCriado = await _usuarioRepository.CriarUsuarioAsync(usuario);
            if (!usuarioCriado) return Resultado<UsuarioRequest>.Falha(UsuarioResource.ErroInesperadoGravacao);

            var usuarioBd = await _usuarioRepository.ObterUsuarioPorCodigoAsync(usuario.Codigo);

            if (!request.Senha.IsNullOrEmpty())
            {
                await _usuarioIdentityRepository.RegistrarContaDeUsuarioRepositoryAsync(
                    request.Codigo.ToUpper(),
                    request.Email,
                    request.Senha);
            }

            if (!request.DepartamentoCodigo.IsNullOrEmpty() && !request.CargoCodigo.IsNullOrEmpty())
            {
                var departamento = await _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsyncAsNoTracking(request.DepartamentoCodigo);
                var cargo = await _cargoRepository.ObterCargoPorCodigoAsync(request.CargoCodigo);

                if (departamento != null && cargo != null)
                {
                    await _usuarioCargoDepartamentoRepository.GravarAssociacaoUsuarioCargoDepartamento(usuarioBd, cargo, departamento);
                }
            }

            await EnviarEmailBoasVindasAsync(request.Email, request.Nome);

            var context = new NotificationBag();
            context.MensagemRegistroSalvo($"Usuário {request.Nome} {request.Sobrenome}");
            
            var resultadoSucesso = Resultado<UsuarioRequest>.Sucesso(request);
            foreach(var msg in context.Messages) resultadoSucesso.Adicionar(msg);
            return resultadoSucesso;
        }

        #endregion

        #region Atualização

        public async Task<Resultado<UsuarioRequest>> AtualizarAsync(UsuarioRequest request)
        {
            var messages = _validador.Validar(request);

            if (messages.Any())
            {
                 var resultado = Resultado<UsuarioRequest>.Falha(messages.FirstOrDefault());
                 foreach(var msg in messages.Skip(1)) resultado.Adicionar(msg);
                 return resultado;
            }
            var usuario = await _usuarioRepository.ObterUsuarioPorCodigoAsync(request.Codigo);
            if (usuario == null)
            {
                var bag = new NotificationBag();
                bag.MensagemRegistroNaoEncontrado(request.Codigo);
                var res = Resultado<UsuarioRequest>.Falha(bag.Messages.FirstOrDefault());
                return res;
            }

            await _mapService.MapToEntityAsync(request, usuario);

            var atualizado = await _usuarioRepository.AtualizarUsuarioAsync(usuario);
            if (!atualizado)
            {
                return Resultado<UsuarioRequest>.Falha(UsuarioResource.ErroInesperadoAtualizacao);
            }

            if (!request.Senha.IsNullOrEmpty())
            {
                await _usuarioIdentityRepository.AtualizarUserIdentityRepositoryAsync(
                    usuario.Codigo,
                    usuario.Email,
                    request.Senha);
            }
            
            if (!request.DepartamentoCodigo.IsNullOrEmpty() && !request.CargoCodigo.IsNullOrEmpty())
            {
                var departamento = await _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsyncAsNoTracking(request.DepartamentoCodigo);
                var cargo = await _cargoRepository.ObterCargoPorCodigoAsync(request.CargoCodigo);

                if (departamento != null && cargo != null)
                {
                    var relacionamento = new UsuarioCargoDepartamento
                    {
                        UsuarioId = usuario.Id,
                        UsuarioCodigo = usuario.Codigo,
                        CargoId = cargo.Id,
                        CargoCodigo = cargo.Codigo,
                        DepartamentoId = departamento.Id,
                        DepartamentoCodigo = departamento.Codigo
                    };

                    await _usuarioCargoDepartamentoRepository.AtualizarRepositoryAsync(relacionamento);
                }
            }

            var context = new NotificationBag();
            context.MensagemRegistroAtualizado(UsuarioContexto);
            var resultadoSucesso = Resultado<UsuarioRequest>.Sucesso(request);
            foreach(var msg in context.Messages) resultadoSucesso.Adicionar(msg);
            return resultadoSucesso;
        }

        #endregion

        #region Consultas

        public async Task<Resultado<List<UsuarioDTO>>> ObterTodosAsync()
        {
            var entities = await _usuarioRepository.ObterUsuariosAsync();
            var dtos = await _asyncMap.MapToListDTOAsync([.. entities]);
            return Resultado<List<UsuarioDTO>>.Sucesso(dtos);
        }

        public async Task<Resultado<UsuarioDTO>> ObterPorCodigoAsync(string codigo)
        {
            if (codigo.IsNullOrEmpty()) return Resultado<UsuarioDTO>.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var entidade = await _usuarioRepository.ObterUsuarioPorCodigoAsync(codigo);

            if (entidade == null) return Resultado<UsuarioDTO>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            var dto = await _asyncMap.MapToDTOAsync(entidade);
            return Resultado<UsuarioDTO>.Sucesso(dto);

        }

        #endregion

        #region Remoção

        public async Task<Resultado> RemoverAsync(string codigo)
        {
            var usuario = await _usuarioRepository.ObterUsuarioPorCodigoAsync(codigo);

            if (usuario == null)
            {
                var bag = new NotificationBag();
                bag.MensagemRegistroNaoEncontrado(codigo);
                var res = Resultado.Falha(bag.Messages.FirstOrDefault());
                return res;
            }

            // Remover tarefas do usuário - Alterar para manter as tarefas, mas desvincular do usuário, se necessário
            var tarefasDoUsuario = await _tarefaRepository.ObterTodasTarefasPorUsuario(usuario.Id, usuario.Codigo);

            foreach (var tarefa in tarefasDoUsuario)
            {
                await _tarefaRepository.RemoverRepositoryAsync(tarefa);
            }

            // Remover salário do usuário - Alterar para manter o histórico salarial, mas desvincular do usuário, se necessário
            var salarioDoUsuario = await _salarioRepository.ObterSalarioPorUsuario(usuario.Id, usuario.Codigo);
            if (salarioDoUsuario != null)
            {
                await _salarioRepository.RemoverRepositoryAsync(salarioDoUsuario);
            }

            // Remover relacionamento cargo/departamento
            var associacao = await _usuarioCargoDepartamentoRepository.ObterPorChaveDoUsuario(usuario.Id, usuario.Codigo);
            if (associacao != null)
            {
                await _usuarioCargoDepartamentoRepository.RemoverRepositoryAsync(associacao);
            }

            // Remover usuário
            await _usuarioRepository.RemoverUsuarioAsync(usuario);

            var context = new NotificationBag();
            context.MensagemRegistroRemovido(UsuarioContexto);
            var resultadoSucesso = Resultado.Sucesso();
            foreach(var msg in context.Messages) resultadoSucesso.Adicionar(msg);
            return resultadoSucesso;
        }

        #endregion

        #region Métodos Privados

        private async Task EnviarEmailBoasVindasAsync(string destinatario, string nomeUsuario)
        {
            if (string.IsNullOrEmpty(destinatario)) return;

            try
            {
                var emailRequest = CriarEmailBoasVindas(destinatario, nomeUsuario);
                await _emailService.EnviarAsync(emailRequest);
            }
            catch
            {
                // Log ou tratamento de erro - não interrompe o fluxo de criação
            }
        }

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
                                        <p>&copy; {DateTime.Now.Year} Sistema Atron. Todos os direitos reservados.</p>
                                    </div>
                                </div>
                            </body>
                            </html>";

            return new EmailRequest
            {
                EmailsDestino = [destinatario],
                Assunto = assunto,
                Mensagem = corpo
            };
        }

        #endregion
    }
}