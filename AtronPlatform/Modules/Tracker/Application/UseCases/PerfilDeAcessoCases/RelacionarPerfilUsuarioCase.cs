using Application.DTO;
using Application.Interfaces.Services;
using Application.Resources;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.Interfaces.Mapping;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Infrastructure.Repositories;
using Shared.Application.Interfaces;

namespace Application.UseCases.PerfilDeAcessoCases
{
    public class RelacionarPerfilUsuarioCase(
        IPerfilDeAcessoUsuarioRepository perfilDeAcessoUsuarioRepository,
        IUsuarioRepository usuarioRepository,
        IToEntityMapper<PerfilDeAcesso, PerfilDeAcessoDTO> map,
        IPerfilDeAcessoRepository perfilDeAcessoRepository,
        IPerfilDeAcessoCacheInvalidator cacheInvalidator,
        IUnitOfWork unitOfWork)
    {
        private readonly IPerfilDeAcessoUsuarioRepository _perfilDeAcessoUsuarioRepository = perfilDeAcessoUsuarioRepository;
        private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;
        private readonly IToEntityMapper<PerfilDeAcesso, PerfilDeAcessoDTO> _map = map;
        private readonly IPerfilDeAcessoRepository _perfilDeAcessoRepository = perfilDeAcessoRepository;
        private readonly IPerfilDeAcessoCacheInvalidator _cacheInvalidator = cacheInvalidator;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Resultado> ExecutarAsync(PerfilDeAcessoUsuarioDTO perfilDeAcessoUsuario)
        {
            var validacao = ValidarComando(perfilDeAcessoUsuario);
            if (validacao.TeveFalha)
                return validacao;

            var perfilRelacionado = await _perfilDeAcessoRepository
                .ObterPerfilPorCodigoRepositoryAsync(perfilDeAcessoUsuario.PerfilDeAcesso.Codigo);
            if (perfilRelacionado is null)
                return Resultado.Falha(MensagemRegistroNaoEncontrado(PerfilDeAcessoResource.Descricao_PerfilDeAcesso));

            var perfilDeAcesso = _map.MapToEntity(perfilDeAcessoUsuario.PerfilDeAcesso);
            var novosRelacionamentos = await PrepararRelacionamentosAsync(perfilDeAcesso, perfilRelacionado, perfilDeAcessoUsuario.Usuarios);
            if (novosRelacionamentos.TeveFalha)
                return Resultado.Falha(novosRelacionamentos.Messages);

            var usuariosAfetados = perfilRelacionado.PerfisDeAcessoUsuario?
                .Select(relacionamento => relacionamento.UsuarioCodigo ?? relacionamento.Usuario?.Codigo)
                .Concat(novosRelacionamentos.Dados.Select(relacionamento => relacionamento.UsuarioCodigo))
                .ToList() ?? novosRelacionamentos.Dados.Select(relacionamento => relacionamento.UsuarioCodigo).ToList();

            foreach (var relacionamentoAtual in perfilRelacionado.PerfisDeAcessoUsuario ?? [])
                await _perfilDeAcessoUsuarioRepository.DeletarRelacionamento(relacionamentoAtual);

            foreach (var novoRelacionamento in novosRelacionamentos.Dados)
            {
                if (!await _perfilDeAcessoUsuarioRepository.CriarPerfilRepositoryAsync(novoRelacionamento))
                    return Resultado.Falha(PerfilDeAcessoResource.Erro_RelacionarUsuarios);
            }

            var commitSucesso = await _unitOfWork.CommitAsync();
            if (!commitSucesso)
                return Resultado.Falha("Falha ao salvar as alterações no banco de dados.");

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
                NotificacoesPadronizadas.Erro_RegistroComDescricaoNaoEncontrado,
                descricao);
        }
    }
}



