using AtronStock.Application.DTO.Request;
using AtronStock.Domain.Entities;
using Shared.Application.Interfaces.Service;
using Shared.Application.Services.Mapper;

namespace AtronStock.Application.Mapping
{
    public class ClienteMapping : AsyncApplicationMapService<ClienteRequest, Cliente>, IAsyncMap<ClienteRequest, Cliente>
    {
        public override Task<ClienteRequest> MapToDTOAsync(Cliente entity)
        {
            var clienteRequest = new ClienteRequest()
            {
                Codigo = entity.Codigo,
                Nome = entity.Nome,
                CPF = entity.CPF,
                CNPJ = entity.CNPJ,
                Email = entity.Email,
                Telefone = entity.Telefone,                             
                EnderecoVO = entity.Endereco
            };           

            return Task.FromResult(clienteRequest);
        }

        public override Task<Cliente> MapToEntityAsync(ClienteRequest dto)
        {
            var entity = new Cliente()
            {
                Codigo = dto.Codigo,
                Nome = dto.Nome,
                CPF = dto.CPF,
                CNPJ = dto.CNPJ,
                Email = dto.Email,
                Telefone = dto.Telefone,               
                Endereco = dto.EnderecoVO
            };

            return Task.FromResult(entity);
        }

        public Task MapToEntityAsync(ClienteRequest dto, Cliente entityToUpdate)
        {
            entityToUpdate.Nome = dto.Nome;
            entityToUpdate.CPF = dto.CPF;
            entityToUpdate.CNPJ = dto.CNPJ;
            entityToUpdate.Email = dto.Email;
            entityToUpdate.Telefone = dto.Telefone;
            entityToUpdate.Endereco = dto.EnderecoVO;

            return Task.CompletedTask;
        }
    }
}
