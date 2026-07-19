using Application.DTO;
using Application.Interfaces.Services;
using Application.Resources;
using Domain.Entities;
using Domain.Interfaces;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services.EntitiesServices
{
    public class PerfilDeAcessoService : IPerfilDeAcessoService
    {
        private readonly IAsyncApplicationMapService<PerfilDeAcessoDTO, PerfilDeAcesso> _map;
        private readonly IPerfilDeAcessoRepository _perfilDeAcessoRepository;
        private readonly IPerfilDeAcessoPreparacaoService _preparacaoService;
        private readonly IPerfilDeAcessoUsuarioSincronizacaoService _sincronizacaoService;
        private readonly IPerfilDeAcessoCacheInvalidator _cacheInvalidator;

        public PerfilDeAcessoService(
            IAsyncApplicationMapService<PerfilDeAcessoDTO, PerfilDeAcesso> map,
            IPerfilDeAcessoRepository perfilDeAcessoRepository,
            IPerfilDeAcessoPreparacaoService preparacaoService,
            IPerfilDeAcessoUsuarioSincronizacaoService sincronizacaoService,
            IPerfilDeAcessoCacheInvalidator cacheInvalidator)
        {
            _map = map;
            _perfilDeAcessoRepository = perfilDeAcessoRepository;
            _preparacaoService = preparacaoService;
            _sincronizacaoService = sincronizacaoService;
            _cacheInvalidator = cacheInvalidator;
        }

        public async Task<Resultado<List<PerfilDeAcessoDTO>>> ObterTodosAsync()
        {
            var perfis = await ObterTodosPerfisServiceAsync();
            return Resultado<List<PerfilDeAcessoDTO>>.Sucesso(perfis.ToList());
        }

        public async Task<Resultado<PerfilDeAcessoDTO>> ObterPorCodigoAsync(string codigo)
        {
            var perfil = await ObterPerfilPorCodigoServiceAsync(codigo);
            return perfil is null
                ? Resultado<PerfilDeAcessoDTO>.Falha(PerfilDeAcessoResource.Erro_RegistroNaoEncontrado)
                : Resultado<PerfilDeAcessoDTO>.Sucesso(perfil);
        }

        public async Task<Resultado<PerfilDeAcessoDTO>> CriarAsync(PerfilDeAcessoDTO perfilDeAcessoDTO)
        {
            var preparacao = await _preparacaoService.PrepararAsync(perfilDeAcessoDTO);
            if (preparacao.TeveFalha)
                return Resultado<PerfilDeAcessoDTO>.Falhas(preparacao.Messages);

            var criado = await _perfilDeAcessoRepository.CriarPerfilRepositoryAsync(preparacao.Dados);
            return criado
                ? Resultado<PerfilDeAcessoDTO>.Sucesso(perfilDeAcessoDTO)
                    .AdicionarMensagem(string.Format(PerfilDeAcessoResource.Mensagem_PerfilCriado, preparacao.Dados.Codigo))
                : Resultado<PerfilDeAcessoDTO>.Falha(PerfilDeAcessoResource.Erro_CriarPerfil);
        }

        public async Task<Resultado<PerfilDeAcessoDTO>> AtualizarAsync(string codigo, PerfilDeAcessoDTO perfilDeAcessoDTO)
        {
            var preparacao = await _preparacaoService.PrepararAsync(perfilDeAcessoDTO);
            if (preparacao.TeveFalha)
                return Resultado<PerfilDeAcessoDTO>.Falhas(preparacao.Messages);

            var perfilAtual = await _perfilDeAcessoRepository.ObterPerfilPorCodigoRepositoryAsync(codigo);
            var atualizado = await _perfilDeAcessoRepository.AtualizarPerfilRepositoryAsync(codigo, preparacao.Dados);
            if (!atualizado)
                return Resultado<PerfilDeAcessoDTO>.Falha(PerfilDeAcessoResource.Erro_AtualizarPerfil);

            _cacheInvalidator.InvalidarUsuariosDoPerfil(perfilAtual);
            return Resultado<PerfilDeAcessoDTO>.Sucesso(perfilDeAcessoDTO)
                .AdicionarMensagem(string.Format(PerfilDeAcessoResource.Mensagem_PerfilAtualizado, preparacao.Dados.Codigo));
        }

        public async Task<Resultado> RemoverAsync(string codigo)
        {
            var perfil = await _perfilDeAcessoRepository.ObterPerfilPorCodigoRepositoryAsync(codigo);
            if (perfil is null)
                return Resultado.Falha(PerfilDeAcessoResource.Erro_RegistroNaoEncontrado);

            var removido = await _perfilDeAcessoRepository.DeletarPerfilRepositoryAsync(perfil);
            if (!removido)
                return Resultado.Falha(PerfilDeAcessoResource.Erro_RemoverPerfil);

            _cacheInvalidator.InvalidarUsuariosDoPerfil(perfil);
            return Resultado.Sucesso(PerfilDeAcessoResource.Mensagem_PerfilRemovido);
        }

        public async Task<Resultado<PerfilDeAcessoUsuarioDTO>> RelacionarPerfilDeAcessoUsuarioAsync(PerfilDeAcessoUsuarioDTO perfilDeAcessoUsuario)
        {
            var resultado = await _sincronizacaoService.SincronizarAsync(perfilDeAcessoUsuario);
            return resultado.TeveFalha
                ? Resultado<PerfilDeAcessoUsuarioDTO>.Falhas(resultado.Messages)
                : Resultado<PerfilDeAcessoUsuarioDTO>.Sucesso(perfilDeAcessoUsuario);
        }

        public async Task<Resultado<PerfilDeAcessoUsuarioDTO>> ObterRelacionamentoDePerfilUsuarioPorCodigoAsync(string codigo)
        {
            var relacionamento = await ObterRelacionamentoDePerfilUsuarioPorCodigoServiceAsync(codigo);
            return relacionamento is null
                ? Resultado<PerfilDeAcessoUsuarioDTO>.Falha(PerfilDeAcessoResource.Erro_RegistroNaoEncontrado)
                : Resultado<PerfilDeAcessoUsuarioDTO>.Sucesso(relacionamento);
        }

        public async Task<ICollection<PerfilDeAcessoDTO>> ObterTodosPerfisServiceAsync()
        {
            var entities = await _perfilDeAcessoRepository.ObterTodosPerfisRepositoryAsync();
            return await _map.MapToListDTOAsync(entities.ToList());
        }

        public async Task<PerfilDeAcessoDTO> ObterPerfilPorIdServiceAsync(int id)
        {
            var entidade = await _perfilDeAcessoRepository.ObterPerfilPorIdRepositoryAsync(id);
            return entidade is null ? null : await _map.MapToDTOAsync(entidade);
        }

        public async Task<PerfilDeAcessoDTO> ObterPerfilPorCodigoServiceAsync(string codigo)
        {
            var entidade = await _perfilDeAcessoRepository.ObterPerfilPorCodigoRepositoryAsync(codigo);
            return entidade is null ? null : await _map.MapToDTOAsync(entidade);
        }

        public async Task<List<PerfilDeAcessoDTO>> ObterPerfisPorCodigoUsuarioServiceAsync(string usuarioCodigo)
        {
            var perfis = await _perfilDeAcessoRepository.ObterPerfisPorCodigoDeUsuarioRepositoryAsync(usuarioCodigo);
            return perfis is null ? null : await _map.MapToListDTOAsync(perfis);
        }

        public async Task<bool> CriarPerfilServiceAsync(PerfilDeAcessoDTO perfilDeAcessoDTO)
        {
            return (await CriarAsync(perfilDeAcessoDTO)).TeveSucesso;
        }

        public async Task<bool> AtualizarPerfilServiceAsync(string codigo, PerfilDeAcessoDTO perfilDeAcessoDTO)
        {
            return (await AtualizarAsync(codigo, perfilDeAcessoDTO)).TeveSucesso;
        }

        public async Task<bool> DeletarPerfilServiceAsync(string codigo)
        {
            return (await RemoverAsync(codigo)).TeveSucesso;
        }

        public async Task<bool> RelacionarPerfilDeAcessoUsuarioServiceAsync(PerfilDeAcessoUsuarioDTO perfilDeAcessoUsuario)
        {
            return (await RelacionarPerfilDeAcessoUsuarioAsync(perfilDeAcessoUsuario)).TeveSucesso;
        }

        public async Task<PerfilDeAcessoUsuarioDTO> ObterRelacionamentoDePerfilUsuarioPorCodigoServiceAsync(string codigo)
        {
            if (string.IsNullOrEmpty(codigo))
                return new PerfilDeAcessoUsuarioDTO();

            var perfilDeAcesso = await _perfilDeAcessoRepository.ObterPerfilPorCodigoRepositoryAsync(codigo);
            if (perfilDeAcesso is null)
                return null;

            var perfilDeAcessoDTO = await _map.MapToDTOAsync(perfilDeAcesso);
            var dto = new PerfilDeAcessoUsuarioDTO
            {
                PerfilDeAcesso = new PerfilDeAcessoDTO
                {
                    Codigo = perfilDeAcessoDTO.Codigo,
                    Descricao = perfilDeAcessoDTO.Descricao,
                    Modulos = perfilDeAcessoDTO.Modulos.ToList()
                }
            };

            foreach (var relacionamento in perfilDeAcesso.PerfisDeAcessoUsuario ?? [])
            {
                dto.Usuarios.Add(new UsuarioDTO
                {
                    Codigo = relacionamento.Usuario.Codigo,
                    Nome = relacionamento.Usuario.Nome,
                    Sobrenome = relacionamento.Usuario.Sobrenome
                });
            }

            return dto;
        }
    }
}
