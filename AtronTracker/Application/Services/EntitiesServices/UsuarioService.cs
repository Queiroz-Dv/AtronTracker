using Application.DTO;
using Application.DTO.Request;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.Identity;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.DTOS.Requests;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services.EntitiesServices
{
    /// <summary>
    /// Serviço de usuário refatorado seguindo o padrão CategoriaService.
    /// Gerencia operações CRUD de usuários de forma padronizada.
    /// </summary>
    public class UsuarioService : IUsuarioService
    {
        private const string UsuarioContexto = nameof(Usuario);

        private readonly IAsyncApplicationMapService<UsuarioDTO, UsuarioIdentity> _mapIdentity;
        private readonly IValidador<UsuarioRequest> _validador;
        private readonly IAsyncMap<UsuarioRequest, Usuario> _mapService;

        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUsuarioCargoDepartamentoRepository _usuarioCargoDepartamentoRepository;
        private readonly IDepartamentoRepository _departamentoRepository;
        private readonly ICargoRepository _cargoRepository;
        private readonly ITarefaRepository _tarefaRepository;
        private readonly ISalarioRepository _salarioRepository;
        private readonly IUsuarioIdentityRepository _usuarioIdentityRepository;
        private readonly IEmailService _emailService;

        public UsuarioService(
            IAsyncApplicationMapService<UsuarioDTO, UsuarioIdentity> mapIdentity,
            IValidador<UsuarioRequest> validador,
            IAsyncMap<UsuarioRequest, Usuario> mapService,
            IUsuarioRepository repository,
            IUsuarioCargoDepartamentoRepository usuarioCargoDepartamentoRepository,
            IDepartamentoRepository departamentoRepository,
            ICargoRepository cargoRepository,
            ITarefaRepository tarefaRepository,
            ISalarioRepository salarioRepository,
            IUsuarioIdentityRepository usuarioIdentityRepository,
            IEmailService emailService)
        {
            _mapIdentity = mapIdentity;
            _usuarioRepository = repository;
            _validador = validador;
            _mapService = mapService;
            _usuarioCargoDepartamentoRepository = usuarioCargoDepartamentoRepository;
            _departamentoRepository = departamentoRepository;
            _cargoRepository = cargoRepository;
            _tarefaRepository = tarefaRepository;
            _salarioRepository = salarioRepository;
            _usuarioIdentityRepository = usuarioIdentityRepository;
            _emailService = emailService;
        }

        #region Criação

        public async Task<Resultado> CriarAsync(UsuarioRequest request)
        {
            // Validação do request
            var messages = _validador.Validar(request);
            if (messages.Any()) return Resultado.Falha(messages);

            // Verificar se código já existe
            var usuarioExistente = await _usuarioRepository.ObterUsuarioPorCodigoAsync(request.Codigo.ToUpper());
            if (usuarioExistente != null)
            {
                return Resultado.Falha("Código de usuário informado já existe. Tente outro novamente.");
            }

            // Verificar se email já existe
            if (!string.IsNullOrEmpty(request.Email))
            {
                var emailExiste = await _usuarioRepository.VerificarEmailExistenteAsync(request.Email);
                if (emailExiste)
                {
                    return Resultado.Falha("O e-mail informado já está cadastrado no sistema.");
                }
            }

            // Mapear para entidade
            var usuario = await _mapService.MapToEntityAsync(request);

            // Criar usuário no banco
            var usuarioCriado = await _usuarioRepository.CriarUsuarioAsync(usuario);
            if (!usuarioCriado)
            {
                return Resultado.Falha("Erro inesperado ao criar usuário.");
            }

            // Buscar usuário criado para obter ID
            var usuarioBd = await _usuarioRepository.ObterUsuarioPorCodigoAsync(usuario.Codigo);

            // Registrar conta de identity se senha foi fornecida
            if (!request.Senha.IsNullOrEmpty())
            {
                await _usuarioIdentityRepository.RegistrarContaDeUsuarioRepositoryAsync(
                    request.Codigo.ToUpper(),
                    request.Email,
                    request.Senha);
            }

            // Criar relacionamento com cargo/departamento se fornecido
            if (!request.DepartamentoCodigo.IsNullOrEmpty() && !request.CargoCodigo.IsNullOrEmpty())
            {
                var departamento = await _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsyncAsNoTracking(request.DepartamentoCodigo);
                var cargo = await _cargoRepository.ObterCargoPorCodigoAsync(request.CargoCodigo);

                if (departamento != null && cargo != null)
                {
                    await _usuarioCargoDepartamentoRepository.GravarAssociacaoUsuarioCargoDepartamento(usuarioBd, cargo, departamento);
                }
            }

            // Enviar e-mail de boas-vindas
            await EnviarEmailBoasVindasAsync(request.Email, request.Nome);

            var context = new NotificationBag();
            context.MensagemRegistroSalvo($"Usuário {request.Nome} {request.Sobrenome}");
            return Resultado.Sucesso(request, [.. context.Messages]);
        }

        #endregion

        #region Atualização

        public async Task<Resultado> AtualizarAsync(UsuarioRequest request)
        {
            // Validação do request
            var messages = _validador.Validar(request);
            if (messages.Any()) return Resultado.Falha(messages);

            // Buscar usuário existente
            var usuario = await _usuarioRepository.ObterUsuarioPorCodigoAsync(request.Codigo);
            if (usuario == null)
            {
                var bag = new NotificationBag();
                bag.MensagemRegistroNaoEncontrado(request.Codigo);
                return Resultado.Falha(bag.Messages.ToList());
            }

            // Mapear dados do request para a entidade existente
            await _mapService.MapToEntityAsync(request, usuario);

            // Atualizar no banco
            var atualizado = await _usuarioRepository.AtualizarUsuarioAsync(request.Codigo, usuario);
            if (!atualizado)
            {
                return Resultado.Falha("Erro inesperado ao atualizar usuário.");
            }

            // Atualizar identity se senha foi fornecida
            if (!request.Senha.IsNullOrEmpty())
            {
                await _usuarioIdentityRepository.AtualizarUserIdentityRepositoryAsync(
                    usuario.Codigo,
                    usuario.Email,
                    request.Senha);
            }

            // Atualizar relacionamento com cargo/departamento se fornecido
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
            return Resultado.Sucesso(request, [.. context.Messages]);
        }

        #endregion

        #region Consultas

        public async Task<Resultado<ICollection<UsuarioRequest>>> ObterTodosRequestAsync()
        {
            var usuarios = await _usuarioRepository.ObterTodosRepositoryAsync();
            var dtos = await _mapService.MapToListDTOAsync(usuarios);
            return Resultado.Sucesso<ICollection<UsuarioRequest>>(dtos);
        }

        public async Task<Resultado<UsuarioDTO>> ObterPorCodigoRequestAsync(string codigo)
        {
            var usuario = await _usuarioRepository.ObterUsuarioPorCodigoAsync(codigo);
            if (usuario == null)
            {
                var bag = new NotificationBag();
                bag.MensagemRegistroNaoEncontrado(codigo);
                return Resultado.Falha<UsuarioDTO>(bag.Messages.ToList());
            }

            var dto = await _mapIdentity.MapToDTOAsync(usuario);
            return Resultado.Sucesso(dto);
        }

        /// <summary>
        /// Mantido para compatibilidade com LoginService e outros serviços existentes.
        /// </summary>
        public async Task<UsuarioDTO> ObterPorCodigoAsync(string codigo)
        {
            var usuario = await _usuarioRepository.ObterUsuarioPorCodigoAsync(codigo);
            return usuario is null ? null : await _mapIdentity.MapToDTOAsync(usuario);
        }

        /// <summary>
        /// Mantido para compatibilidade com outros serviços existentes.
        /// </summary>
        public async Task<List<UsuarioDTO>> ObterTodosAsync()
        {
            var usuarios = await _usuarioRepository.ObterTodosUsuariosDoIdentity();
            return await _mapIdentity.MapToListDTOAsync(usuarios);
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
                return Resultado.Falha(bag.Messages.ToList());
            }

            // Remover tarefas do usuário
            var tarefasDoUsuario = await _tarefaRepository.ObterTodasTarefasPorUsuario(usuario.Id, usuario.Codigo);
            foreach (var tarefa in tarefasDoUsuario)
            {
                await _tarefaRepository.RemoverRepositoryAsync(tarefa);
            }

            // Remover salário do usuário
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
            return Resultado.Sucesso([.. context.Messages]);
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
                EmailsDestino = new List<string> { destinatario },
                Assunto = assunto,
                Mensagem = corpo
            };
        }

        #endregion
    }
}