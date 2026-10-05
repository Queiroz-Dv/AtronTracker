using Application.DTO;
using Application.UseCases.UsuarioCases;
using Domain.Interfaces;
using Shared.Domain.ValueObjects;

namespace Application.UseCases.TarefaCases
{
    public sealed class ObterAcessoTarefaCase(
        ObterUsuarioCase usuarioService,
        ITarefaRepository tarefaRepository)
    {
        private readonly ObterUsuarioCase _usuarioService = usuarioService;
        private readonly ITarefaRepository _tarefaRepository = tarefaRepository;

        public async Task<Resultado<TarefaAcessoDTO>> ExecutarAsync()
        {
            var usuarioResultado = await _usuarioService.ObterAsync();
            if (usuarioResultado.TeveFalha)
                return Resultado<TarefaAcessoDTO>.Falhas(usuarioResultado.Messages);

            var usuario = usuarioResultado.Dados;
            var possuiResponsabilidadeGestao = await _tarefaRepository
                .PossuiResponsabilidadeGestaoAsync(usuario.Id, usuario.Codigo);

            return Resultado<TarefaAcessoDTO>.Sucesso(new TarefaAcessoDTO
            {
                PossuiResponsabilidadeGestao = possuiResponsabilidadeGestao
            });
        }
    }
}