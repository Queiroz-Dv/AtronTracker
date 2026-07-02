using Application.DTO;
using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.DTOS.Requests;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Application.Services.EntitiesServices
{
    public class TarefaService : ITarefaService
    {
        private readonly IAsyncApplicationMapService<TarefaDTO, Tarefa> _map;
        private readonly ITarefaRepository _tarefaRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IValidador<TarefaDTO> _validador;
        private readonly IEmailService _emailService;

        public TarefaService(
            IAsyncApplicationMapService<TarefaDTO, Tarefa> map,
            ITarefaRepository tarefaRepository,
            IUsuarioRepository usuarioRepository,
            IValidador<TarefaDTO> validador,
            IEmailService emailService)
        {
            _map = map;
            _tarefaRepository = tarefaRepository;
            _usuarioRepository = usuarioRepository;
            _validador = validador;
            _emailService = emailService;
        }

        public async Task<Resultado<TarefaDTO>> CriarAsync(TarefaDTO tarefaDTO)
        {
            var erros = _validador.Validar(tarefaDTO);
            if (erros.Any())
                return Resultado<TarefaDTO>.Falhas(erros);

            var usuario = await _usuarioRepository.ObterUsuarioPorCodigoAsync(tarefaDTO.UsuarioCodigo.ToUpper());
            if (usuario is null)
                return Resultado<TarefaDTO>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            var tarefa = await _map.MapToEntityAsync(tarefaDTO);
            VincularUsuario(tarefa, usuario);

            var gravado = await _tarefaRepository.CriarTarefaAsync(tarefa);
            if (!gravado)
                return Resultado<TarefaDTO>.Falha("Erro ao gravar a tarefa.");

            tarefaDTO.Id = tarefa.Id;
            var resultado = Resultado<TarefaDTO>
                .Sucesso(tarefaDTO)
                .AdicionarMensagem("Tarefa gravada com sucesso.");

            var envioEmail = await EnviarNotificacaoCriacaoAsync(tarefaDTO, usuario);
            if (envioEmail.TeveFalha)
            {
                resultado.AdicionarAviso("Tarefa criada, mas não foi possível enviar o e-mail de notificação.");
            }

            return resultado;
        }

        public async Task<Resultado<List<TarefaDTO>>> ObterTodosAsync()
        {
            var tarefas = await _tarefaRepository.ObterTodasTarefas();
            var dtos = await _map.MapToListDTOAsync(tarefas);
            return Resultado<List<TarefaDTO>>.Sucesso(dtos);
        }

        public Task<Resultado<List<TarefaEstadoDTO>>> ObterEstadosAsync()
        {
            return Task.FromResult(Resultado<List<TarefaEstadoDTO>>.Sucesso(TarefaEstadoDTO.TarefasEstados()));
        }

        public async Task<Resultado<TarefaDTO>> AtualizarAsync(int id, TarefaDTO tarefaDTO)
        {
            var tarefaExistente = await _tarefaRepository.ObterTarefaPorId(id);
            if (tarefaExistente is null)
                return Resultado<TarefaDTO>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            var erros = _validador.Validar(tarefaDTO);
            if (erros.Any())
                return Resultado<TarefaDTO>.Falhas(erros);

            var usuario = await _usuarioRepository.ObterUsuarioPorCodigoAsync(tarefaDTO.UsuarioCodigo.ToUpper());
            if (usuario is null)
                return Resultado<TarefaDTO>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            var tarefa = await _map.MapToEntityAsync(tarefaDTO);
            VincularUsuario(tarefa, usuario);

            var atualizado = await _tarefaRepository.AtualizarTarefaAsync(id, tarefa);
            if (!atualizado)
                return Resultado<TarefaDTO>.Falha("Erro ao atualizar a tarefa.");

            tarefaDTO.Id = id;
            return Resultado<TarefaDTO>
                .Sucesso(tarefaDTO)
                .AdicionarMensagem("Tarefa atualizada com sucesso.");
        }

        public async Task<Resultado> ExcluirAsync(string id)
        {
            if (!int.TryParse(id, out var tarefaId))
                return Resultado.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var tarefa = await _tarefaRepository.ObterTarefaPorId(tarefaId);
            if (tarefa is null)
                return Resultado.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            var deletado = await _tarefaRepository.RemoverRepositoryAsync(tarefa);
            if (!deletado)
                return Resultado.Falha("Erro ao remover a tarefa.");

            return Resultado
                .Sucesso()
                .AdicionarMensagem("Tarefa removida com sucesso.");
        }

        public async Task<Resultado<TarefaDTO>> ObterPorId(int id)
        {
            var tarefaRepository = await _tarefaRepository.ObterTarefaPorId(id);
            if (tarefaRepository is null)
                return Resultado<TarefaDTO>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            var dto = await _map.MapToDTOAsync(tarefaRepository);
            return Resultado<TarefaDTO>.Sucesso(dto);
        }

        private static void VincularUsuario(Tarefa tarefa, Usuario usuario)
        {
            tarefa.UsuarioId = usuario.Id;
            tarefa.UsuarioCodigo = usuario.Codigo;
        }

        private async Task<Resultado> EnviarNotificacaoCriacaoAsync(TarefaDTO tarefa, Usuario usuario)
        {
            if (!usuario.ReceberNotificacaoTarefaPorEmail || usuario.Email.IsNullOrEmpty())
            {
                return Resultado.Sucesso();
            }

            var mensagem = new EmailRequest
            {
                EmailsDestino = [usuario.Email],
                Assunto = $"Nova tarefa atribuída: {tarefa.Titulo}",
                Mensagem = GerarCorpoEmailTarefa(tarefa, usuario)
            };

            return await _emailService.EnviarAsync(mensagem);
        }

        private static string GerarCorpoEmailTarefa(TarefaDTO tarefa, Usuario usuario)
        {
            var nomeUsuario = WebUtility.HtmlEncode($"{usuario.Nome} {usuario.Sobrenome}".Trim());
            var titulo = WebUtility.HtmlEncode(tarefa.Titulo);
            var conteudo = WebUtility.HtmlEncode(tarefa.Conteudo ?? string.Empty);
            var estado = WebUtility.HtmlEncode(ObterDescricaoEstado(tarefa));

            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f4f4f4; }}
        .container {{ max-width: 640px; margin: 0 auto; background-color: #ffffff; padding: 28px; border-radius: 8px; }}
        .header {{ border-bottom: 2px solid #007bff; padding-bottom: 16px; }}
        .header h1 {{ color: #007bff; margin: 0; font-size: 22px; }}
        .content {{ padding: 18px 0; color: #333; line-height: 1.5; }}
        .task-box {{ background-color: #f8f9fa; border-left: 4px solid #007bff; padding: 14px; margin-top: 14px; }}
        .task-box p {{ margin: 6px 0; }}
        .footer {{ border-top: 1px solid #eee; padding-top: 14px; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Nova tarefa atribuída</h1>
        </div>
        <div class='content'>
            <p>Olá, <strong>{nomeUsuario}</strong>.</p>
            <p>Uma nova tarefa foi atribuída a você no Sistema Atron.</p>
            <div class='task-box'>
                <p><strong>Título:</strong> {titulo}</p>
                <p><strong>Conteúdo:</strong> {conteudo}</p>
                <p><strong>Data inicial:</strong> {tarefa.DataInicial:dd/MM/yyyy}</p>
                <p><strong>Data final:</strong> {tarefa.DataFinal:dd/MM/yyyy}</p>
                <p><strong>Estado inicial:</strong> {estado}</p>
            </div>
        </div>
        <div class='footer'>
            <p>Este é um e-mail automático do Sistema Atron.</p>
        </div>
    </div>
</body>
</html>";
        }

        private static string ObterDescricaoEstado(TarefaDTO tarefa)
        {
            if (tarefa.EstadoDaTarefa is not null && !tarefa.EstadoDaTarefa.Descricao.IsNullOrEmpty())
            {
                return tarefa.EstadoDaTarefa.Descricao;
            }

            return TarefaEstadoDTO
                .TarefasEstados()
                .FirstOrDefault(estado => estado.Id == tarefa.EstadoDaTarefa?.Id)
                ?.Descricao ?? "Não informado";
        }
    }
}
