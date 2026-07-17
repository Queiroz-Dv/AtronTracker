using Domain.Interfaces;
using Domain.Interfaces.UsuarioInterfaces;
using Application.Interfaces.Services;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.UseCases.Usuario
{
    /// <summary>
    /// Caso de uso responsável pela remoção de um usuário.
    /// A conta Identity NÃO é removida — dívida técnica aceita conscientemente.
    /// </summary>
    public class RemoverUsuario
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUsuarioCargoDepartamentoRepository _usuarioCargoDepartamentoRepository;
        private readonly ITarefaRepository _tarefaRepository;
        private readonly ICacheUsuarioService _cacheUsuarioService;

        public RemoverUsuario(
            IUsuarioRepository usuarioRepository,
            IUsuarioCargoDepartamentoRepository usuarioCargoDepartamentoRepository,
            ITarefaRepository tarefaRepository,
            ICacheUsuarioService cacheUsuarioService)
        {
            _usuarioRepository = usuarioRepository;
            _usuarioCargoDepartamentoRepository = usuarioCargoDepartamentoRepository;
            _tarefaRepository = tarefaRepository;
            _cacheUsuarioService = cacheUsuarioService;
        }

        public async Task<Resultado> ExecutarAsync(string codigo)
        {
            // 1. Verificação de existência
            var usuario = await _usuarioRepository
                .ObterUsuarioPorCodigoAsync(codigo);

            if (usuario == null)
                return Resultado
                    .Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            // 2. Remoção das tarefas do usuário
            //    DÍVIDA TÉCNICA: Hard delete mantido intencionalmente.
            //    Tarefas devem ter Soft Delete no futuro (histórico deve ser preservado).
            //    Aguarda decisão de escopo mais amplo antes de implementar.
            var tarefasDoUsuario = await _tarefaRepository
                .ObterTodasTarefasPorUsuario(usuario.Id, usuario.Codigo);

            foreach (var tarefa in tarefasDoUsuario)
            {
                await _tarefaRepository.RemoverRepositoryAsync(tarefa);
            }

            // 3. Remoção da associação Cargo / Departamento
            var associacao = await _usuarioCargoDepartamentoRepository
                .ObterPorChaveDoUsuario(usuario.Id, usuario.Codigo);

            if (associacao != null)
            {
                await _usuarioCargoDepartamentoRepository
                    .RemoverRepositoryAsync(associacao);
            }

            // 4. Remoção do usuário de negócio
            await _usuarioRepository.RemoverUsuarioAsync(usuario);
            _cacheUsuarioService.RemoverCacheDeAcessoTokenInfo(usuario.Codigo);

            // DÍVIDA TÉCNICA: A conta Identity NÃO é removida intencionalmente.
            // Motivação: Soft Delete depende de mapeamento EF + migration ainda não implementados.
            // Risco aceito: usuário removido do negócio ainda consegue autenticar via Identity
            // até que o Soft Delete seja implementado. Monitorar impacto até resolução.

            // 5. Retorno padronizado
            return Resultado
                .Sucesso()
                .AdicionarMensagem(UsuarioResource.MensagemUsuarioRemovido);
        }
    }
}
