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
        protected readonly MessageModel _messageModel; // Modelo de notificações e validações para a entidade

        public ModuleController(Service service, MessageModel messageModel)
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

        protected virtual ActionResult RetornoPadrao<DTO>(DTO dto)
        {
            if (dto is null)
            {
                return _messageModel.Messages.HasErrors() ?
                BadRequest(ObterNotificacoes()) :
                Ok(ObterNotificacoes());
            }
            else
            {
                return _messageModel.Messages.HasErrors() ?
                     BadRequest(ObterNotificacoes()) :
                     Ok(dto);
            }
        }
    }
}