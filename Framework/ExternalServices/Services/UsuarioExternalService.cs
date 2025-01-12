using Atron.Application.DTO;
using Atron.Domain.Entities;
using Communication.Extensions;
using Communication.Interfaces;
using Communication.Interfaces.Services;
using Communication.Models;
using ExternalServices.Interfaces;
using ExternalServices.Interfaces.ApiRoutesInterfaces;
using Newtonsoft.Json;
using Shared.Extensions;
using Shared.Models;

namespace ExternalServices.Services
{
    /// <summary>
    /// Classe que implementa os processos e fluxo do módulo de Usuários
    /// </summary>
    public class UsuarioExternalService : IUsuarioExternalService
    {
        private readonly IApiClient _client;
        private readonly ICommunicationService _communicationService;
        private readonly IUrlModuleFactory _urlModuleFactory;
        private MessageModel<Usuario> _messageModel;

        public UsuarioExternalService(IApiClient apiClient,
            ICommunicationService communicationService,
            IUrlModuleFactory urlModuleFactory,
            MessageModel<Usuario> messageModel)
        {
            _client = apiClient;
            _communicationService = communicationService;
            _urlModuleFactory = urlModuleFactory;
            _messageModel = messageModel;
        }

        public async Task<List<UsuarioDTO>> ObterTodos()
        {
            var response = await _client.GetAsync(_urlModuleFactory.Url);

            return JsonConvert.DeserializeObject<List<UsuarioDTO>>(response);
        }

        public async Task Criar(UsuarioDTO usuarioDTO)
        {
            RemontarEntidade(usuarioDTO);
            var json = JsonConvert.SerializeObject(usuarioDTO);
            await _client.PostAsync(_urlModuleFactory.Url, json);
            _messageModel.Messages.AddMessages(_communicationService);
        }

        private static void RemontarEntidade(UsuarioDTO usuarioDTO)
        {
            usuarioDTO.Codigo = usuarioDTO.Codigo.ToUpper();
        }

        public async Task<UsuarioDTO> ObterPorCodigo(string codigo)
        {
            if (codigo.IsNullOrEmpty())
            {
                _messageModel.AddRegisterInvalidMessage(nameof(Usuario));
                return null;
            }

            var response = await _client.GetAsync(_urlModuleFactory.Url);
            return JsonConvert.DeserializeObject<UsuarioDTO>(response);
        }

        public async Task Atualizar(string codigoUsuario, UsuarioDTO usuario)
        {
            if (codigoUsuario.IsNullOrEmpty() || usuario.Codigo.IsNullOrEmpty())
            {
                _messageModel.AddRegisterInvalidMessage(nameof(Usuario));
                return;
            }

            var usuarioJson = JsonConvert.SerializeObject(usuario);

            try
            {
                await _client.PutAsync(_urlModuleFactory.Url, codigoUsuario, usuarioJson);
                _messageModel.Messages.AddMessages(_communicationService);
            }
            catch (HttpRequestException ex)
            {
                var exceptionMessage = JsonConvert.DeserializeObject<List<Message>>(ex.Message);
                if (exceptionMessage is not null)
                {
                    _messageModel.Messages.AddRange(exceptionMessage);
                }
                else
                {
                    _messageModel.AddError(ex.Message);
                }
            }
            catch(JsonSerializationException ex)
            {
                _messageModel.AddError(ex.Message);
            }
            catch (Exception ex)
            {
                _messageModel.AddError(ex.Message);
            }
        }

        public async Task Remover(string codigo)
        {
            if (string.IsNullOrEmpty(codigo))
            {
                _messageModel.AddRegisterInvalidMessage(nameof(Usuario));
                return;
            }

            await _client.DeleteAsync(_urlModuleFactory.Url, codigo);
            _messageModel.Messages.AddMessages(_communicationService);
        }
    }
}