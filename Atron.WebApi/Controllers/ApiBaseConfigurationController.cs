using Microsoft.AspNetCore.Mvc;
using Shared.Extensions;
using Shared.Models;
using System.Collections.Generic;

namespace Atron.WebApi.Controllers
{
    // Qual o objetivo da controller de módulos?
    // Centralizar operações repetitivas e segregar responsabilidades da controller dos módulos   
    //[Produces("application/json")]
    public class ApiBaseConfigurationController<Entity, Service> : ControllerBase
    {
        protected readonly Service _service; // Serviço da entidade
        protected readonly MessageModel _messageModel; // Modelo de notificações e validações para a entidade

        public ApiBaseConfigurationController(Service service, MessageModel messageModel)
        {
            // Injeta as dependências necessárias para os processos automatizados
            _service = service;
            _messageModel = messageModel;
        }

        // Serve para retornar um objeto dinâmico (aqui estou retornando a conversão em json)
        protected virtual IEnumerable<dynamic> ObterNotificacoes()
        {
            // Passar para um método de extensão
            return _messageModel.Messages.ConvertMessageToJson();
        }
    }
}