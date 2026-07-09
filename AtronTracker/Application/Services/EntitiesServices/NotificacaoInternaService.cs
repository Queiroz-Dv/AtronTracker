using Application.DTO;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services.EntitiesServices
{
    public class NotificacaoInternaService : INotificacaoInternaService
    {
        private readonly INotificacaoInternaRepository _notificacaoInternaRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUserAccessor _userAccessor;

        public NotificacaoInternaService(
            INotificacaoInternaRepository notificacaoInternaRepository,
            IUsuarioRepository usuarioRepository,
            IUserAccessor userAccessor)
        {
            _notificacaoInternaRepository = notificacaoInternaRepository;
            _usuarioRepository = usuarioRepository;
            _userAccessor = userAccessor;
        }

        public async Task<Resultado<List<NotificacaoInternaDTO>>> ObterMinhasAsync()
        {
            var usuario = await ObterUsuarioLogadoAsync();
            if (usuario.TeveFalha)
                return Resultado<List<NotificacaoInternaDTO>>.Falhas(usuario.Messages);

            var notificacoes = await _notificacaoInternaRepository.ObterPorUsuarioAsync(
                usuario.Dados.Id,
                usuario.Dados.Codigo);

            return Resultado<List<NotificacaoInternaDTO>>.Sucesso(
                notificacoes.Select(Mapear).ToList());
        }

        public async Task<Resultado<NotificacaoInternaDTO>> MarcarComoLidaAsync(int id)
        {
            var usuario = await ObterUsuarioLogadoAsync();
            if (usuario.TeveFalha)
                return Resultado<NotificacaoInternaDTO>.Falhas(usuario.Messages);

            var atualizada = await _notificacaoInternaRepository.MarcarComoLidaAsync(
                id,
                usuario.Dados.Id,
                usuario.Dados.Codigo);

            if (!atualizada)
                return Resultado<NotificacaoInternaDTO>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            var notificacao = await _notificacaoInternaRepository.ObterPorIdEUsuarioAsync(
                id,
                usuario.Dados.Id,
                usuario.Dados.Codigo);

            return Resultado<NotificacaoInternaDTO>
                .Sucesso(Mapear(notificacao))
                .AdicionarMensagem("Notificação marcada como lida.");
        }

        public async Task<Resultado<List<NotificacaoInternaDTO>>> MarcarTodasComoLidasAsync()
        {
            var usuario = await ObterUsuarioLogadoAsync();
            if (usuario.TeveFalha)
                return Resultado<List<NotificacaoInternaDTO>>.Falhas(usuario.Messages);

            var atualizadas = await _notificacaoInternaRepository.MarcarTodasComoLidasAsync(
                usuario.Dados.Id,
                usuario.Dados.Codigo);

            if (!atualizadas)
                return Resultado<List<NotificacaoInternaDTO>>.Falha("Não foi possível marcar as notificações como lidas.");

            var notificacoes = await _notificacaoInternaRepository.ObterPorUsuarioAsync(
                usuario.Dados.Id,
                usuario.Dados.Codigo);

            return Resultado<List<NotificacaoInternaDTO>>
                .Sucesso(notificacoes.Select(Mapear).ToList())
                .AdicionarMensagem("Notificações marcadas como lidas.");
        }

        public async Task<Resultado<NotificacaoInternaDTO>> CriarAsync(NotificacaoInterna notificacao)
        {
            notificacao.DataCriacao = notificacao.DataCriacao == default
                ? System.DateTime.Now
                : notificacao.DataCriacao;

            var gravada = await _notificacaoInternaRepository.CriarAsync(notificacao);
            if (!gravada)
                return Resultado<NotificacaoInternaDTO>.Falha("Não foi possível criar a notificação interna.");

            return Resultado<NotificacaoInternaDTO>.Sucesso(Mapear(notificacao));
        }

        private async Task<Resultado<Usuario>> ObterUsuarioLogadoAsync()
        {
            var usuarioCodigo = _userAccessor.ObterCodigoUsuarioLogado();
            if (usuarioCodigo.IsNullOrEmpty())
                return Resultado<Usuario>.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var usuario = await _usuarioRepository.ObterUsuarioPorCodigoAsync(usuarioCodigo);
            if (usuario is null)
                return Resultado<Usuario>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            return Resultado<Usuario>.Sucesso(usuario);
        }

        private static NotificacaoInternaDTO Mapear(NotificacaoInterna notificacao)
        {
            return new NotificacaoInternaDTO
            {
                Id = notificacao.Id,
                Titulo = notificacao.Titulo,
                Mensagem = notificacao.Mensagem,
                Modulo = notificacao.Modulo,
                TipoEvento = notificacao.TipoEvento,
                UrlDestino = notificacao.UrlDestino,
                TarefaId = notificacao.TarefaId,
                Lida = notificacao.Lida,
                DataCriacao = notificacao.DataCriacao,
                DataLeitura = notificacao.DataLeitura
            };
        }
    }
}
