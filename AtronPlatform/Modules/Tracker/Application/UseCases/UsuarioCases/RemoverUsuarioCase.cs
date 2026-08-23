using Domain.Interfaces;
using Domain.Interfaces.Identity;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.DTOS.Common;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Application.UseCases.UsuarioCases
{
    public class RemoverUsuarioCase(
        IUsuarioRepository usuarioRepository,
        IUsuarioCargoDepartamentoRepository usuarioCargoDepartamentoRepository,
        ITarefaRepository tarefaRepository,
        IUsuarioIdentityRepository usuarioIdentityRepository,
        IAuditoriaService auditoriaService)
    {        

        public async Task<Resultado> ExecutarAsync(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return Resultado.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var usuario = await usuarioRepository.ObterUsuarioPorCodigoAsync(codigo);
            if (usuario is null)
                return Resultado.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            var tarefas = (await tarefaRepository
                    .ObterTodasTarefasPorUsuario(usuario.Id, usuario.Codigo))
                .ToList();
            var associacao = await usuarioCargoDepartamentoRepository
                .ObterPorChaveDoUsuario(usuario.Id, usuario.Codigo);

            if (tarefas.Count > 0 && associacao is null)
                return Resultado.Falha(UsuarioResource.ErroRepassarTarefasUsuario);

            var identity = await usuarioIdentityRepository.ObterUsuarioIdentityPorCodigo(usuario.Codigo);
            var deletado = !await usuarioIdentityRepository.DeletarContaUserRepositoryAsync(usuario.Codigo);

            if (identity is not null && deletado)
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

            if (associacao is not null)
                await usuarioCargoDepartamentoRepository.RemoverRepositoryAsync(associacao);

            if (!await usuarioRepository.RemoverUsuarioAsync(usuario))
                return Resultado.Falha(UsuarioResource.ErroRemoverUsuario);

            await auditoriaService.RemoverServiceAsync(new AuditoriaDTO
            {
                CodigoRegistro = usuario.Codigo,
                Contexto = nameof(Domain.Entities.Usuario),
                Historico = new HistoricoDTO
                {
                    CodigoRegistro = usuario.Codigo,
                    Contexto = nameof(Domain.Entities.Usuario),
                    Descricao = $"Usuário {usuario.Codigo} removido em {DateTime.Now:dd/MM/yyyy HH:mm}."
                }
            });

            return Resultado.Sucesso().AdicionarMensagem(UsuarioResource.MensagemUsuarioRemovido);
        }
    }
}
