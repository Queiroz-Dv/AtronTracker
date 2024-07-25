using Atron.Application.DTO;
using Atron.Application.ViewInterfaces;
using Atron.Domain.Entities;
using Atron.Domain.ViewsInterfaces;
using AutoMapper;
using Notification.Models;
using Shared.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Application.ViewServices
{
    public class DepartamentoViewService : NotificationWebViewModel, IDepartamentoViewService
    {
        private IMapper _mapper;
        private readonly IDepartamentoViewRepository _viewRepository;

        public List<NotificationMessage> _messages { get; }

        public DepartamentoViewService(IDepartamentoViewRepository viewRepository, IMapper mapper)
        {
            _mapper = mapper;
            _viewRepository = viewRepository;
            _messages = new List<NotificationMessage>();
        }

        public async Task CriarDepartamento(DepartamentoDTO departamento)
        {
            var entidade = _mapper.Map<Departamento>(departamento);

            await _viewRepository.CriarDepartamento(entidade);
        }

        private void ObterMensagesRepository()
        {
            _messages.AddRange(_viewRepository._messages);
        }

        public async Task<List<DepartamentoDTO>> ObterDepartamentos()
        {
            var departamentosDict = await _viewRepository.GetDepartamentosAsync();

            if (departamentosDict == null)
            {
                return null;
            }

            var departamentos = new List<DepartamentoDTO>();
            foreach (var dict in departamentosDict)
            {
                var departamento = new DepartamentoDTO
                {
                    Codigo = dict.ContainsKey("codigo") ? dict["codigo"].ToString() : null,
                    Descricao = dict.ContainsKey("descricao") ? dict["descricao"].ToString() : null
                };
                departamentos.Add(departamento);
            }

            return departamentos;
        }

        public ApiWebViewMessageResponse GetJsonResponseContent()
        {
            return GetResultResponse();
        }
    }
}