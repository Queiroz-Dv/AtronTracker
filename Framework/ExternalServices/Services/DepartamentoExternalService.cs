using Atron.Application.DTO;
using Atron.Domain.ApiEntities;
using Atron.Domain.Entities;
using Communication.Interfaces;
using Communication.Interfaces.Services;
using ExternalServices.Interfaces;
using ExternalServices.Interfaces.ApiRoutesInterfaces;
using Newtonsoft.Json;
using Shared.DTO;
using Shared.Enums;
using Shared.Models;

namespace ExternalServices.Services
{
    /// <summary>
    /// Classe que implementa o processo e fluxo de departamentos
    /// </summary>
    public class DepartamentoExternalService : IDepartamentoExternalService
    {
        private readonly IApiClient _apiClient;
        private readonly ICommunicationService _communicationService;
        private readonly IApiRouteExternalService _apiRouteExternalService;
        private readonly MessageModel<Departamento> _messageModel;

        public List<ResultResponseDTO> ResultResponses { get; set; }


        public string Uri { get; set; }

        public string Modulo { get; set; }

        public DepartamentoExternalService(
            MessageModel<Departamento> messageModel,
            IApiClient apiClient, 
            ICommunicationService communicationService, 
            IApiRouteExternalService apiRouteExternalService)
        {
            _apiRouteExternalService = apiRouteExternalService;
            _apiClient = apiClient;
            _communicationService = communicationService;
            _messageModel = messageModel;
        }



        public async Task Atualizar(string codigo, DepartamentoDTO departamentoDTO)
        {
            var json = JsonConvert.SerializeObject(departamentoDTO);

            if (string.IsNullOrEmpty(codigo) || string.IsNullOrEmpty(departamentoDTO.Codigo)) 
            {
                _messageModel.AddRegisterInvalidMessage(nameof(Departamento));
                //_communicationService.AddMessage(new Message() { Description = "Código não informado", Level = MessageLevel.Error });
                return;
            }

            try
            {
                var routa = await _apiRouteExternalService.MontarRotaDoModulo(Uri, Modulo);
                string uri = routa.BuildUriRoute();
                uri += $"/{string.Empty}";

                await _apiClient.PutAsync(uri, codigo, json);
            }
            catch (HttpRequestException ex)
            {
                var exceptionMessage = JsonConvert.DeserializeObject<List<Message>>(ex.Message);
                _communicationService.AddMessages(exceptionMessage);
            }
            catch (Exception ex)
            {
                _communicationService.AddMessage(new Message() { Description = ex.Message, Level = MessageLevel.Error });
            }
        }

        public async Task<DepartamentoDTO> ObterPorCodigo(string codigo)
        {
            if (string.IsNullOrEmpty(codigo))
            {
                NewMethod();
                return new DepartamentoDTO();
            }

            var rotaDepartamento = await _apiRouteExternalService.MontarRotaDoModulo(Uri, Modulo);
            string uri = rotaDepartamento.BuildUriRoute();
            //melhorar essa parte
            uri += $"/{codigo}";
            var response = await _apiClient.GetAsync(uri);
            var departamento = JsonConvert.DeserializeObject<DepartamentoDTO>(response);
            return departamento;
        }

        private void NewMethod()
        {
            _communicationService.AddMessage(new Message() { Description = "Código não foi informado.", Level = MessageLevel.Error });
        }

        public async Task Criar(DepartamentoDTO departamento)
        {
            var routes = await _apiRouteExternalService.MontarRotaDoModulo(Uri, Modulo);
            string uri = routes.BuildUriRoute();

            var json = JsonConvert.SerializeObject(departamento);
            await _apiClient.PostAsync(uri, json);
        }

        public async Task<List<DepartamentoDTO>> ObterTodos()
        {
            var rotasDepartamento = await _apiRouteExternalService.MontarRotaDoModulo(Uri, Modulo);
            string uri = rotasDepartamento.BuildUriRoute();

            var response = await _apiClient.GetAsync(uri);

            var departamentos = JsonConvert.DeserializeObject<List<DepartamentoDTO>>(response);
            return departamentos is not null ? departamentos : new List<DepartamentoDTO>();
        }

        public async Task Remover(string codigo)
        {
            if (string.IsNullOrEmpty(codigo))
            {

            }
            await _apiClient.DeleteAsync("https://atron-hmg.azurewebsites.net/api/Departamento/ExcluirDepartamento/", codigo);
            ResultResponses.AddRange(_communicationService.GetResultResponses());
        }

        public IList<Message> GetMessages()
        {
            return _communicationService.GetMessages();
        }
    }

    public static class BuildModuleRouting
    {
        public static string BuildUriRoute(this ApiRoute route)
        {
            return route is null ? null : $"{route.Url}/{route.Modulo}";
        }
    }
}