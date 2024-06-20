using Atron.Application.DTO;
using Atron.Application.Interfaces;
using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using AutoMapper;
using Notification.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Application.Services
{
    public class DepartamentoService : IDepartamentoService
    {
        /*  A Inversão de Controle consiste em não criar uma instância do repositório
         *  diretamente no construtor, mas utilizar um contêiner para lidar com isso. 
         *  Em outras palavras, você inverte o controle da criação e gerenciamento
         *  das dependências do serviço para um contêiner de IoC. 
         *  Em vez de o DepartamentoService controlar a criação do DepartamentoRepository, 
         *  essa responsabilidade é delegada ao contêiner de IoC. 
         *  O serviço depende de uma abstração (IDepartamentoRepository) 
         *  em vez de uma implementação concreta (DepartamentoRepository).*/
        private readonly IMapper _mapper;
        private readonly IDepartamentoRepository _departamentoRepository;
        private readonly NotificationModel<Departamento> _notification;

        public DepartamentoService(IMapper mapper, IDepartamentoRepository departamentoRepository,
            NotificationModel<Departamento> notification)
        {
            /* A Injeção de Dependência via construtor é usada para fornecer 
             * a dependência ao DepartamentoService. 
             * Isso é feito passando apenas a abstração (IDepartamentoRepository) 
             * sem instanciar a classe concreta dentro do serviço.
             * Isso permite que o DepartamentoService não precise saber
             * como o repositório executa suas funções, promovendo o desacoplamento e a testabilidade. */
            _mapper = mapper;
            _departamentoRepository = departamentoRepository;
            _notification = notification;
        }

        public string Ambiente { get; set; }

        public void AtualizarAsync(DepartamentoDTO departmentDTO)
        {
            throw new System.NotImplementedException();
        }

        public void CriarAsync(DepartamentoDTO departmentDTO)
        {
            throw new System.NotImplementedException();
        }

        public Task<DepartamentoDTO> ObterPorCodigo(string codigo)
        {
            throw new System.NotImplementedException();
        }

        public Task<DepartamentoDTO> ObterPorIdAsync(int? departamentoId)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IEnumerable<DepartamentoDTO>> ObterTodosAsync()
        {
            var entities = await _departamentoRepository.ObterDepartmentosAsync();
            var dtos = _mapper.Map<List<DepartamentoDTO>>(entities);
            return dtos;
        }

        public void RemoverAsync(string codigo)
        {
            throw new System.NotImplementedException();
        }
    }
}
