using Application.DTO;
using Application.Interfaces.Services;
using Application.Resources;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services.EntitiesServices
{
    public class PerfilDeAcessoService : IPerfilDeAcessoService
    {
        private readonly IAsyncApplicationMapService<PerfilDeAcessoDTO, PerfilDeAcesso> _map;
        private readonly IPerfilDeAcessoRepository _perfilDeAcessoRepository;
        private readonly IPerfilDeAcessoUsuarioRepository _perfilDeAcessoUsuarioRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IModuloRepository _moduloRepository;
        private readonly IValidateModelService<PerfilDeAcesso> _validateModel;
        private readonly ICacheUsuarioService _cacheUsuarioService;
        private readonly Notifiable _messageModel;

        public PerfilDeAcessoService(
            IPerfilDeAcessoUsuarioRepository perfilDeAcessoUsuarioRepository,
            IUsuarioRepository usuarioRepository,
            IAsyncApplicationMapService<PerfilDeAcessoDTO, PerfilDeAcesso> map,
            IPerfilDeAcessoRepository perfilDeAcessoRepository,          
            IModuloRepository moduloRepository,
            IValidateModelService<PerfilDeAcesso> validateModel,
            ICacheUsuarioService cacheUsuarioService,
            Notifiable messageModel)
        {
            _map = map;
            _usuarioRepository = usuarioRepository;
            _perfilDeAcessoUsuarioRepository = perfilDeAcessoUsuarioRepository;
            _perfilDeAcessoRepository = perfilDeAcessoRepository;
            _moduloRepository = moduloRepository;
            _validateModel = validateModel;
            _cacheUsuarioService = cacheUsuarioService;
            _messageModel = messageModel;
        }

        public async Task<Resultado<List<PerfilDeAcessoDTO>>> ObterTodosAsync()
        {
            var perfis = await ObterTodosPerfisServiceAsync();
            return Resultado<List<PerfilDeAcessoDTO>>.Sucesso(perfis.ToList());
        }

        public async Task<Resultado<PerfilDeAcessoDTO>> ObterPorCodigoAsync(string codigo)
        {
            var perfil = await ObterPerfilPorCodigoServiceAsync(codigo);

            return perfil is null ?
                Resultado<PerfilDeAcessoDTO>.Falha(PerfilDeAcessoResource.Erro_RegistroNaoEncontrado) :
                Resultado<PerfilDeAcessoDTO>.Sucesso(perfil);
        }

        public async Task<Resultado<PerfilDeAcessoDTO>> CriarAsync(PerfilDeAcessoDTO perfilDeAcessoDTO)
        {
            var criado = await CriarPerfilServiceAsync(perfilDeAcessoDTO);
            return MontarResultado(criado, perfilDeAcessoDTO, PerfilDeAcessoResource.Erro_CriarPerfil);
        }

        public async Task<Resultado<PerfilDeAcessoDTO>> AtualizarAsync(string codigo, PerfilDeAcessoDTO perfilDeAcessoDTO)
        {
            var atualizado = await AtualizarPerfilServiceAsync(codigo, perfilDeAcessoDTO);
            return MontarResultado(atualizado, perfilDeAcessoDTO, PerfilDeAcessoResource.Erro_AtualizarPerfil);
        }

        public async Task<Resultado> RemoverAsync(string codigo)
        {
            var removido = await DeletarPerfilServiceAsync(codigo);
            return MontarResultado(removido, PerfilDeAcessoResource.Erro_RemoverPerfil);
        }

        public async Task<Resultado<PerfilDeAcessoUsuarioDTO>> RelacionarPerfilDeAcessoUsuarioAsync(PerfilDeAcessoUsuarioDTO perfilDeAcessoUsuario)
        {
            var relacionado = await RelacionarPerfilDeAcessoUsuarioServiceAsync(perfilDeAcessoUsuario);
            return MontarResultado(relacionado, perfilDeAcessoUsuario, PerfilDeAcessoResource.Erro_RelacionarUsuarios);
        }

        public async Task<Resultado<PerfilDeAcessoUsuarioDTO>> ObterRelacionamentoDePerfilUsuarioPorCodigoAsync(string codigo)
        {
            var relacionamento = await ObterRelacionamentoDePerfilUsuarioPorCodigoServiceAsync(codigo);

            return _messageModel.Notificacoes.HasErrors() ?
                Resultado<PerfilDeAcessoUsuarioDTO>.Falhas(_messageModel.Notificacoes) :
                Resultado<PerfilDeAcessoUsuarioDTO>.Sucesso(relacionamento);
        }

        private Resultado<T> MontarResultado<T>(bool teveSucesso, T dados, string mensagemErroPadrao)
        {
            if (teveSucesso)
                return Resultado<T>.Sucesso(dados, _messageModel.Notificacoes);

            if (!_messageModel.Notificacoes.HasErrors())
                _messageModel.AdicionarErro(mensagemErroPadrao);

            return Resultado<T>.Falhas(_messageModel.Notificacoes);
        }

        private Resultado MontarResultado(bool teveSucesso, string mensagemErroPadrao)
        {
            if (teveSucesso)
                return Resultado.Sucesso(_messageModel.Notificacoes);

            if (!_messageModel.Notificacoes.HasErrors())
                _messageModel.AdicionarErro(mensagemErroPadrao);

            return Resultado.Falha(_messageModel.Notificacoes);
        }

        private void ChecarPerfilModulo(PerfilDeAcessoDTO perfilDeAcessoDTO)
        {
            if (perfilDeAcessoDTO is null)
            {
                _messageModel.AdicionarErro(PerfilDeAcessoResource.Erro_PerfilInvalido);
                return;
            }

            if (perfilDeAcessoDTO.Modulos is null || !perfilDeAcessoDTO.Modulos.Any())
            {
                _messageModel.AdicionarErro(PerfilDeAcessoResource.Erro_SemModulos);
            }
        }


        public async Task<bool> AtualizarPerfilServiceAsync(string codigo, PerfilDeAcessoDTO perfilDeAcessoDTO)
        {
            ChecarPerfilModulo(perfilDeAcessoDTO);

            if (!_messageModel.Notificacoes.HasErrors())
            {
                var usuariosAfetados = await ObterCodigosDosUsuariosPorPerfil(codigo);
                var perfilDeAcesso = await _map.MapToEntityAsync(perfilDeAcessoDTO);

                await PreencherInformacoesDaEntidade(perfilDeAcessoDTO, perfilDeAcesso);

                _validateModel.Validate(perfilDeAcesso);

                if (!_messageModel.Notificacoes.HasErrors())
                {
                    var prf = await _perfilDeAcessoRepository.AtualizarPerfilRepositoryAsync(codigo, perfilDeAcesso);
                    if (prf)
                    {
                        _messageModel.AdicionarMensagem(string.Format(PerfilDeAcessoResource.Mensagem_PerfilAtualizado, perfilDeAcesso.Codigo));
                        InvalidarCacheDosUsuarios(usuariosAfetados);

                        return prf;
                    }
                }

                return false;
            }

            return false;
        }

        private async Task PreencherInformacoesDaEntidade(PerfilDeAcessoDTO perfilDeAcessoDTO, PerfilDeAcesso perfilDeAcesso)
        {
            // Aqui preciso obter o id de cada módulo para relacionar ao perfil e as propriedades
            foreach (var moduloDTO in perfilDeAcessoDTO.Modulos)
            {
                // Cria o objeto global do foreach
                var perfilDeAcessoModulo = new PerfilDeAcessoModulo();

                // Obtém o módulo por código para preencher o id
                var modulo = await _moduloRepository.ObterPorCodigoRepository(moduloDTO.Codigo);

                // Preenche a entidade principal
                perfilDeAcessoModulo.PerfilDeAcessoId = perfilDeAcesso.Id;
                perfilDeAcessoModulo.PerfilDeAcessoCodigo = perfilDeAcesso.Codigo;
                perfilDeAcessoModulo.ModuloId = modulo.Id;
                perfilDeAcessoModulo.ModuloCodigo = modulo.Codigo;

                // Inclui a entidade totalmente preenchida na lista
                perfilDeAcesso.PerfilDeAcessoModulos.Add(perfilDeAcessoModulo);
            }
        }

        // Para criar um perfil eu preciso pelo menos ter um módulo relacionado a ele
        public async Task<bool> CriarPerfilServiceAsync(PerfilDeAcessoDTO perfilDeAcessoDTO)
        {
            ChecarPerfilModulo(perfilDeAcessoDTO);

            if (!_messageModel.Notificacoes.HasErrors())
            {
                var perfilDeAcesso = await _map.MapToEntityAsync(perfilDeAcessoDTO);

                await PreencherInformacoesDaEntidade(perfilDeAcessoDTO, perfilDeAcesso);

                // Lembrar de validar o módulo de acordo com as regras de negócio
                _validateModel.Validate(perfilDeAcesso);

                if (!_messageModel.Notificacoes.HasErrors())
                {
                    var prf = await _perfilDeAcessoRepository.CriarPerfilRepositoryAsync(perfilDeAcesso);
                    if (prf)
                    {
                        _messageModel.AdicionarMensagem(string.Format(PerfilDeAcessoResource.Mensagem_PerfilCriado, perfilDeAcesso.Codigo));

                        return prf;
                    }
                }
                return false;
            }
            return false;
        }

        public async Task<bool> DeletarPerfilServiceAsync(string codigo)
        {
            var perfil = await _perfilDeAcessoRepository.ObterPerfilPorCodigoRepositoryAsync(codigo);
            if (perfil is null)
            {
                _messageModel.MensagemRegistroNaoEncontrado(PerfilDeAcessoResource.Descricao_PerfilDeAcesso);
            }
            else
            {
                var usuariosAfetados = ObterCodigosDosUsuarios(perfil);
                var result = await _perfilDeAcessoRepository.DeletarPerfilRepositoryAsync(perfil);
                _messageModel.AdicionarMensagem(PerfilDeAcessoResource.Mensagem_PerfilRemovido);

                if (result)
                    InvalidarCacheDosUsuarios(usuariosAfetados);

                return result;
            }

            return false;
        }

        public async Task<PerfilDeAcessoDTO> ObterPerfilPorCodigoServiceAsync(string codigo)
        {
            var entidade = await _perfilDeAcessoRepository.ObterPerfilPorCodigoRepositoryAsync(codigo);
            return entidade is null ? null : await _map.MapToDTOAsync(entidade);
        }

        public async Task<PerfilDeAcessoDTO> ObterPerfilPorIdServiceAsync(int id)
        {
            var entidade = await _perfilDeAcessoRepository.ObterPerfilPorIdRepositoryAsync(id);
            return entidade is null ? null : await _map.MapToDTOAsync(entidade);
        }

        public async Task<ICollection<PerfilDeAcessoDTO>> ObterTodosPerfisServiceAsync()
        {
            var entities = await _perfilDeAcessoRepository.ObterTodosPerfisRepositoryAsync();
            return await _map.MapToListDTOAsync(entities.ToList());
        }

        public async Task<bool> RelacionarPerfilDeAcessoUsuarioServiceAsync(PerfilDeAcessoUsuarioDTO dto)
        {
            ChecarPerfilDeAcessoUsuario(dto);

            if (!_messageModel.Notificacoes.HasErrors())
            {
                // Preciso remover antes pois não terei o Update diretamente, basta remover os registros e refazer a gravação
                var perfilRelacionado = await _perfilDeAcessoRepository.ObterPerfilPorCodigoRepositoryAsync(dto.PerfilDeAcesso.Codigo);
                var usuariosAfetados = new List<string>();

                if (perfilRelacionado is null)
                {
                    _messageModel.MensagemRegistroNaoEncontrado(PerfilDeAcessoResource.Descricao_PerfilDeAcesso);
                    return false;
                }

                if (perfilRelacionado.PerfisDeAcessoUsuario.Any())
                {
                    usuariosAfetados.AddRange(ObterCodigosDosUsuarios(perfilRelacionado));

                    foreach (var item in perfilRelacionado.PerfisDeAcessoUsuario)
                    {
                        await _perfilDeAcessoUsuarioRepository.DeletarRelacionamento(item);
                    }
                }

                var perfilDeAcesso = await _map.MapToEntityAsync(dto.PerfilDeAcesso);
                perfilDeAcesso.PerfisDeAcessoUsuario = new List<PerfilDeAcessoUsuario>();
                foreach (var usuarioDTO in dto.Usuarios)
                {
                    usuariosAfetados.Add(usuarioDTO.Codigo);
                    var perfilDeAcessoUsuario = new PerfilDeAcessoUsuario();

                    var usuarioRepo = await _usuarioRepository.ObterUsuarioPorCodigoAsync(usuarioDTO.Codigo);
                    if (usuarioRepo is null)
                    {
                        _messageModel.MensagemRegistroNaoEncontrado(PerfilDeAcessoResource.Descricao_Usuario);
                        return false;
                    }

                    perfilDeAcessoUsuario.UsuarioId = usuarioRepo.Id;
                    perfilDeAcessoUsuario.UsuarioCodigo = usuarioRepo.Codigo;

                    var perfilRepo = await _perfilDeAcessoRepository.ObterPerfilPorCodigoRepositoryAsync(perfilDeAcesso.Codigo);
                    perfilDeAcessoUsuario.PerfilDeAcessoId = perfilRepo.Id;
                    perfilDeAcessoUsuario.PerfilDeAcessoCodigo = perfilRepo.Codigo;

                    perfilDeAcesso.PerfisDeAcessoUsuario.Add(perfilDeAcessoUsuario);
                }


                // Só pra testar a gravação a partir desse ponto
                int salvos = 0;
                foreach (var perfil in perfilDeAcesso.PerfisDeAcessoUsuario)
                {
                    var result = await _perfilDeAcessoUsuarioRepository.CriarPerfilRepositoryAsync(perfil);
                    if (result)
                    {
                        salvos++;
                    }
                }

                var relacionou = salvos > 0;
                if (relacionou)
                    InvalidarCacheDosUsuarios(usuariosAfetados);

                return relacionou;
            }

            return false;
        }

        private void ChecarPerfilDeAcessoUsuario(PerfilDeAcessoUsuarioDTO perfilDeAcessoUsuario)
        {
            if (perfilDeAcessoUsuario is null || perfilDeAcessoUsuario.PerfilDeAcesso is null)
            {
                _messageModel.AdicionarErro(PerfilDeAcessoResource.Erro_PerfilInvalido);
                return;
            }

            if (perfilDeAcessoUsuario.Usuarios is null || !perfilDeAcessoUsuario.Usuarios.Any())
            {
                _messageModel.AdicionarErro(PerfilDeAcessoResource.Erro_SemUsuarios);
            }
        }

        public async Task<PerfilDeAcessoUsuarioDTO> ObterRelacionamentoDePerfilUsuarioPorCodigoServiceAsync(string codigo)
        {
            if (codigo.IsNullOrEmpty())
            {
                return await Task.FromResult(new PerfilDeAcessoUsuarioDTO());
            }

            var perfilDeAcesso = await _perfilDeAcessoRepository.ObterPerfilPorCodigoRepositoryAsync(codigo);
            if (perfilDeAcesso is null)
            {
                _messageModel.MensagemRegistroNaoEncontrado(PerfilDeAcessoResource.Descricao_PerfilDeAcesso);
                return new PerfilDeAcessoUsuarioDTO();
            }

            var perfilDeAcessoDTO = await _map.MapToDTOAsync(perfilDeAcesso);

            var dto = new PerfilDeAcessoUsuarioDTO();

            dto.PerfilDeAcesso.Codigo = perfilDeAcessoDTO.Codigo;
            dto.PerfilDeAcesso.Descricao = perfilDeAcessoDTO.Descricao;

            foreach (var relacionamento in perfilDeAcesso.PerfisDeAcessoUsuario)
            {
                var usuarioDto = new UsuarioDTO
                {
                    Codigo = relacionamento.Usuario.Codigo,
                    Nome = relacionamento.Usuario.Nome,
                    Sobrenome = relacionamento.Usuario.Sobrenome
                };

                dto.Usuarios.Add(usuarioDto);
            }

            foreach (var modulo in perfilDeAcessoDTO.Modulos)
            {
                dto.PerfilDeAcesso.Modulos.Add(modulo);
            }

            return dto;

        }

        public async Task<List<PerfilDeAcessoDTO>> ObterPerfisPorCodigoUsuarioServiceAsync(string usuarioCodigo)
        {
            var perfis = await _perfilDeAcessoRepository.ObterPerfisPorCodigoDeUsuarioRepositoryAsync(usuarioCodigo);

            return perfis != null ? await _map.MapToListDTOAsync(perfis) : null;
        }

        private async Task<List<string>> ObterCodigosDosUsuariosPorPerfil(string codigoPerfil)
        {
            var perfil = await _perfilDeAcessoRepository.ObterPerfilPorCodigoRepositoryAsync(codigoPerfil);
            return ObterCodigosDosUsuarios(perfil);
        }

        private static List<string> ObterCodigosDosUsuarios(PerfilDeAcesso perfil)
        {
            return perfil?.PerfisDeAcessoUsuario?
                .Select(relacionamento => relacionamento.UsuarioCodigo ?? relacionamento.Usuario?.Codigo)
                .Where(codigo => !codigo.IsNullOrEmpty())
                .Distinct()
                .ToList() ?? [];
        }

        private void InvalidarCacheDosUsuarios(IEnumerable<string> codigosUsuarios)
        {
            foreach (var codigoUsuario in codigosUsuarios.Where(codigo => !codigo.IsNullOrEmpty()).Distinct())
            {
                _cacheUsuarioService.RemoverCacheDeAcessoTokenInfo(codigoUsuario);
            }
        }
    }
}
