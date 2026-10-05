using Domain.Interfaces;
using Domain.Interfaces.Identity;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.Messaging;
using Shared.Application.Resources;
using Shared.Domain.Events.Auditoria;
using Shared.Domain.ValueObjects;
using Shared.Extensions;

namespace Application.UseCases.UsuarioCases
{
    public class RemoverUsuarioCase(
        IUsuarioRepository usuarioRepository,
        IUsuarioCargoDepartamentoRepository usuarioCargoDepartamentoRepository,
        ITarefaRepository tarefaRepository,
        IUsuarioIdentityRepository usuarioIdentityRepository,
        IEventBus eventBus)
    {

        public async Task<Resultado> ExecutarAsync(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return Resultado.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var usuario = await usuarioRepository.ObterUsuarioPorCodigoAsync(codigo);
            if (usuario.IsNullable())
                return Resultado.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            var tarefas = (await tarefaRepository
                    .ObterTodasTarefasPorUsuario(usuario.Id, usuario.Codigo))
                .ToList();
            var associacao = await usuarioCargoDepartamentoRepository
                .ObterPorChaveDoUsuario(usuario.Id, usuario.Codigo);

            if (tarefas.Count > 0 && associacao.IsNullable())
                return Resultado.Falha(UsuarioResource.ErroRepassarTarefasUsuario);

            var identity = await usuarioIdentityRepository.UsuarioServiceIdentityPorCodigo(usuario.Codigo);
            var deletado = !await usuarioIdentityRepository.DeletarContaUserRepositoryAsync(usuario.Codigo);

            if (identity.IsNotNull() && deletado)
                return Resultado.Falha(UsuarioResource.ErroRemoverUsuario);

            foreach (var tarefa in tarefas)
            {
                tarefa.UsuarioId = null;
                tarefa.UsuarioCodigo = null;
                tarefa.Usuario = null;
                tarefa.DepartamentoId = associacao!.DepartamentoId;
                tarefa.DepartamentoCodigo = associacao.DepartamentoCodigo;
                tarefa.CargoId = associacao.CargoId;
                tarefa.CargoCodigo = associacao.CargoCodigo;
                tarefa.Cargo = null;

                if (!await tarefaRepository.AtualizarTarefaAsync(tarefa.Id, tarefa))
                    return Resultado.Falha(UsuarioResource.ErroRepassarTarefasUsuario);
            }

            if (associacao.IsNotNull())
                await usuarioCargoDepartamentoRepository.RemoverAssociacaoUsuarioCargoDepartamento(associacao);

            if (!await usuarioRepository.RemoverUsuarioAsync(usuario))
                return Resultado.Falha(UsuarioResource.ErroRemoverUsuario);

            await eventBus.PublicarAsync(new AuditoriaRemovidaEvent(
                usuario.Codigo,
                nameof(Domain.Entities.Usuario),
                "Usuário $({usuario.Codigo}) removido em $({DateTime.Now:dd/MM/yyyy HH:mm})."
            ));

            return Resultado.Sucesso().AdicionarMensagem(UsuarioResource.MensagemUsuarioRemovido);
        }
    }
}