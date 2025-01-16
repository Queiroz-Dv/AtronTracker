using Microsoft.AspNetCore.Mvc;
using Shared.DTO;
using Shared.Interfaces;
using Shared.Models;

namespace Atron.WebViews.Controllers
{
    // Controller que será um conteiner para todos o fluxo
    public abstract class ServiceConteinerController<DTO, Entity, ExternalService> : Controller
    {
        protected IPaginationService<DTO> _paginationService;

        protected ExternalService _service;

        protected MessageModel<Entity> _messageModel;

        protected PageInfoDTO PageInfo { get; set; }

        protected string Filter { get; set; }

        protected string KeyToSearch { get; set; }

        protected bool ForceFilter { get; set; } = true;

        protected string ApiController { get; set; }
        protected string ActionName { get; set; }

        protected ServiceConteinerController(
            IPaginationService<DTO> paginationService,
            ExternalService externalService,
            MessageModel<Entity> messageModel)
        {
            _paginationService = paginationService;
            _service = externalService;
            _messageModel = messageModel;
            PageInfo = new PageInfoDTO();
        }
    }
}