using Atron.Application.DTO;
using Atron.Application.ViewInterfaces;
using Atron.Domain.Entities;
using Atron.Domain.ViewsInterfaces;
using AutoMapper;
using Notification.Enums;
using Notification.Models;
using Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.Application.ViewServices
{
    public class DepartamentoViewService : IDepartamentoViewService
    {
        private IMapper _mapper;
        private readonly IDepartamentoViewRepository _viewRepository;

        public List<NotificationMessage> _messages { get; }

        public List<ResultResponse> ResultApiJson { get; set; }

        public DepartamentoViewService(IDepartamentoViewRepository viewRepository, IMapper mapper)
        {
            _mapper = mapper;
            _viewRepository = viewRepository;
            _messages = new List<NotificationMessage>();
            ResultApiJson = new List<ResultResponse>();
        }

        public async Task CriarDepartamento(DepartamentoDTO departamento)
        {
            var entidade = _mapper.Map<Departamento>(departamento);

            await _viewRepository.CriarDepartamento(entidade);

            FillMessagesService();
        }

        private void FillMessagesService()
        {
            ResultApiJson.AddRange(_viewRepository.ResultApiJson);
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
            List<DepartamentoDTO> departamentos = MontarDepartamentos(departamentosDict);

            return departamentos;
        }

        private static List<DepartamentoDTO> MontarDepartamentos(List<Dictionary<string, object>> departamentosDict)
        {
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

        public async Task<DepartamentoDTO> ObterPorCodigo(string codigo)
        {
            var departamentosApi = await _viewRepository.GetDepartamentosAsync();
            var departamentos = MontarDepartamentos(departamentosApi);

            var departamento = departamentos.FirstOrDefault(dpt => dpt.Codigo == codigo);

            return departamento;
        }      
    }
}