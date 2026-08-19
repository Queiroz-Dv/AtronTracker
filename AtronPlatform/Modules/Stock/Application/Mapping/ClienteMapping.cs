using AtronStock.Application.DTO.Request;
using AtronStock.Domain.Entities;
using Shared.Application.Interfaces.Mapping;
using Shared.Domain.ValueObjects;
using Shared.Extensions;

namespace AtronStock.Application.Mapping
{
    public class ClienteMapping : Mapper<Cliente, ClienteRequest>
    {
        public override ClienteRequest MapToDto(Cliente entity)
        {
            return new ClienteRequest()
            {
                Codigo = entity.Codigo,
                Nome = entity.Nome,
                Documento = new Documento(entity.CPF.IsNullOrEmpty() ? entity.CNPJ : entity.CPF),                
                Email = entity.Email,
                Telefone = entity.Telefone,                             
                StatusPessoa = entity.Status,
                EnderecoVO = entity.Endereco
            };           

        }

        public override Cliente MapToEntity(ClienteRequest dto)
        {
            return new Cliente()
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

        }

        public void MapToEntity(ClienteRequest dto, Cliente entityToUpdate)
        {
            entityToUpdate.Nome = dto.Nome;
            entityToUpdate.CPF = dto.Documento.Dado;
            entityToUpdate.CNPJ = dto.Documento.Dado;
            entityToUpdate.Email = dto.Email;
            entityToUpdate.Telefone = dto.Telefone;
            entityToUpdate.Status = dto.StatusPessoa;
            entityToUpdate.Endereco = dto.EnderecoVO;

        }
    }
}
