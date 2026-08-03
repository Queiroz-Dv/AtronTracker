using AtronStock.Application.DTO.Request;
using AtronStock.Domain.Entities;
using Shared.Application.Interfaces.Service;
using Shared.Application.Services.Mapper;
using Shared.Domain.ValueObjects;
using Shared.Extensions;

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
                Documento = new Documento(entity.CPF.IsNullOrEmpty() ? entity.CNPJ : entity.CPF),                
                Email = entity.Email,
                Telefone = entity.Telefone,                             
                StatusPessoa = entity.Status,
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
                CPF = dto.Documento.Dado,
                CNPJ = dto.Documento.Dado,
                Email = dto.Email,
                Telefone = dto.Telefone,            
                Status = dto.StatusPessoa,
                Endereco = dto.EnderecoVO
            };

            return Task.FromResult(entity);
        }

        public Task MapToEntityAsync(ClienteRequest dto, Cliente entityToUpdate)
        {
            entityToUpdate.Nome = dto.Nome;
            entityToUpdate.CPF = dto.Documento.Dado;
            entityToUpdate.CNPJ = dto.Documento.Dado;
            entityToUpdate.Email = dto.Email;
            entityToUpdate.Telefone = dto.Telefone;
            entityToUpdate.Status = dto.StatusPessoa;
            entityToUpdate.Endereco = dto.EnderecoVO;

            return Task.CompletedTask;
        }
    }
}
