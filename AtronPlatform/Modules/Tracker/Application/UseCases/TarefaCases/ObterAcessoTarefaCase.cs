using Application.DTO;
using Application.Interfaces.Services;
using Domain.Interfaces;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.UseCases.TarefaCases
{
    public sealed class ObterAcessoTarefaCase(
        IUsuarioService usuarioService,
        ITarefaRepository tarefaRepository)
    {
        private readonly IUsuarioService _usuarioService = usuarioService;
        private readonly ITarefaRepository _tarefaRepository = tarefaRepository;

        public async Task<Resultado<TarefaAcessoDTO>> ExecutarAsync()
        {
            var usuarioResultado = await _usuarioService.ObterUsuarioAtual();
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
