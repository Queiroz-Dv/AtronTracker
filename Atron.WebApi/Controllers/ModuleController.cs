using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using System.Collections.Generic;
using Shared.Extensions;

namespace Atron.WebApi.Controllers
{
    // Qual o objetivo da controller de módulos?
    // Centralizar operações repetitivas e segregar responsabilidades da controller dos módulos
    [ApiController]
    public class ModuleController<Entity, Service> : ControllerBase
    {
        protected readonly Service _service; // Serviço da entidade
        protected readonly MessageModel<Entity> _messageModel; // Modelo de mensagem da entidade

        public ModuleController(Service service, MessageModel<Entity> messageModel)
        {
            // Injeta as dependências necessárias para os processos automatizados
            _service = service;
            _messageModel = messageModel;
        }

        // Serve para retornar um objeto dinâmico como modelo json
        protected virtual IEnumerable<dynamic> ObterNotificacoes()
        {
            return _messageModel.Messages.ConvertMessageToJson();
        }
    }
}