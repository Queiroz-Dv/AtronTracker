using Application.DTO;
using Application.Interfaces.Services;
using Application.Resources;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Infrastructure.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services.EntitiesServices
{
    public class PerfilDeAcessoUsuarioSincronizacaoService : IPerfilDeAcessoUsuarioSincronizacaoService
    {
        private readonly IPerfilDeAcessoUsuarioRepository _perfilDeAcessoUsuarioRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IAsyncApplicationMapService<PerfilDeAcessoDTO, PerfilDeAcesso> _map;
        private readonly IPerfilDeAcessoRepository _perfilDeAcessoRepository;
        private readonly IPerfilDeAcessoCacheInvalidator _cacheInvalidator;
        private readonly ITransactionManager _transactionManager;

        public PerfilDeAcessoUsuarioSincronizacaoService(
            IPerfilDeAcessoUsuarioRepository perfilDeAcessoUsuarioRepository,
            IUsuarioRepository usuarioRepository,
            IAsyncApplicationMapService<PerfilDeAcessoDTO, PerfilDeAcesso> map,
            IPerfilDeAcessoRepository perfilDeAcessoRepository,
            IPerfilDeAcessoCacheInvalidator cacheInvalidator,
            ITransactionManager transactionManager)
        {
            _perfilDeAcessoUsuarioRepository = perfilDeAcessoUsuarioRepository;
            _usuarioRepository = usuarioRepository;
            _map = map;
            _perfilDeAcessoRepository = perfilDeAcessoRepository;
            _cacheInvalidator = cacheInvalidator;
            _transactionManager = transactionManager;
        }

        public async Task<Resultado> SincronizarAsync(PerfilDeAcessoUsuarioDTO perfilDeAcessoUsuario)
        {
            var validacao = ValidarComando(perfilDeAcessoUsuario);
            if (validacao.TeveFalha)
                return validacao;

            var perfilRelacionado = await _perfilDeAcessoRepository
                .ObterPerfilPorCodigoRepositoryAsync(perfilDeAcessoUsuario.PerfilDeAcesso.Codigo);
            if (perfilRelacionado is null)
                return Resultado.Falha(MensagemRegistroNaoEncontrado(PerfilDeAcessoResource.Descricao_PerfilDeAcesso));

            var perfilDeAcesso = await _map.MapToEntityAsync(perfilDeAcessoUsuario.PerfilDeAcesso);
            var novosRelacionamentos = await PrepararRelacionamentosAsync(perfilDeAcesso, perfilRelacionado, perfilDeAcessoUsuario.Usuarios);
            if (novosRelacionamentos.TeveFalha)
                return Resultado.Falha(novosRelacionamentos.Messages);

            var usuariosAfetados = perfilRelacionado.PerfisDeAcessoUsuario?
                .Select(relacionamento => relacionamento.UsuarioCodigo ?? relacionamento.Usuario?.Codigo)
                .Concat(novosRelacionamentos.Dados.Select(relacionamento => relacionamento.UsuarioCodigo))
                .ToList() ?? novosRelacionamentos.Dados.Select(relacionamento => relacionamento.UsuarioCodigo).ToList();

            using (var transacao = _transactionManager.CreateScope())
            {
                foreach (var relacionamentoAtual in perfilRelacionado.PerfisDeAcessoUsuario ?? [])
                    await _perfilDeAcessoUsuarioRepository.DeletarRelacionamento(relacionamentoAtual);

                foreach (var novoRelacionamento in novosRelacionamentos.Dados)
                {
                    if (!await _perfilDeAcessoUsuarioRepository.CriarPerfilRepositoryAsync(novoRelacionamento))
                        return Resultado.Falha(PerfilDeAcessoResource.Erro_RelacionarUsuarios);
                }

                transacao.Complete();
            }

            _cacheInvalidator.InvalidarUsuarios(usuariosAfetados);
            return Resultado.Sucesso();
        }

        private async Task<Resultado<List<PerfilDeAcessoUsuario>>> PrepararRelacionamentosAsync(
            PerfilDeAcesso perfilDeAcesso,
            PerfilDeAcesso perfilRelacionado,
            IEnumerable<UsuarioDTO> usuarios)
        {
            var relacionamentos = new List<PerfilDeAcessoUsuario>();

            foreach (var usuarioDTO in usuarios)
            {
                var usuario = await _usuarioRepository.ObterUsuarioPorCodigoAsync(usuarioDTO.Codigo);
                if (usuario is null)
                    return Resultado<List<PerfilDeAcessoUsuario>>.Falha(MensagemRegistroNaoEncontrado(PerfilDeAcessoResource.Descricao_Usuario));

                relacionamentos.Add(new PerfilDeAcessoUsuario
                {
                    UsuarioId = usuario.Id,
                    UsuarioCodigo = usuario.Codigo,
                    PerfilDeAcessoId = perfilRelacionado.Id,
                    PerfilDeAcessoCodigo = perfilDeAcesso.Codigo
                });
            }

            return Resultado<List<PerfilDeAcessoUsuario>>.Sucesso(relacionamentos);
        }

        private static Resultado ValidarComando(PerfilDeAcessoUsuarioDTO perfilDeAcessoUsuario)
        {
            if (perfilDeAcessoUsuario is null || perfilDeAcessoUsuario.PerfilDeAcesso is null)
                return Resultado.Falha(PerfilDeAcessoResource.Erro_PerfilInvalido);

            return perfilDeAcessoUsuario.Usuarios is null || !perfilDeAcessoUsuario.Usuarios.Any()
                ? Resultado.Falha(PerfilDeAcessoResource.Erro_SemUsuarios)
                : Resultado.Sucesso();
        }

        private static string MensagemRegistroNaoEncontrado(string descricao)
        {
            return string.Format(
                NotificacoesPadronizadas.ResourceManager.GetString("Erro_RegistroComDescricaoNaoEncontrado"),
                descricao);
        }
    }
}
