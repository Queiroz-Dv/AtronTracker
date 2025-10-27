using AtronStock.Application.DTO.Request;
using AtronStock.Application.Interfaces;
using AtronStock.Domain.Entities;
using AtronStock.Domain.Interfaces;

namespace AtronStock.Application.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _repository;

        public ClienteService(IClienteRepository repository)
        {
            _repository = repository;
        }


        public async Task CriarAsync(ClienteRequest request)
        {
            var cliente = new Cliente()
            {
                Codigo = request.Codigo,
                Nome = request.Nome,
                CPF = request.CPF,
                CNPJ = request.CNPJ,
                Email = request.Email,
                Telefone = request.Telefone,
                Status = request.StatusPessoa,
            };

            await _repository.CriarCliente(cliente);
        }
    }
}
