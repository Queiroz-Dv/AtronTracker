using AtronStock.Application.DTO.Request;
using AtronStock.Domain.Entities;
using Shared.Application.Interfaces.Service;
using Shared.Application.Services.Mapper;

namespace AtronStock.Application.Mapping
{
    public class FornecedorMapping : AsyncApplicationMapService<FornecedorRequest, Fornecedor>, IAsyncMap<FornecedorRequest, Fornecedor>
    {
        public override Task<FornecedorRequest> MapToDTOAsync(Fornecedor entity)
        {
            var fornecedorRequest = new FornecedorRequest
            {
                Codigo = entity.Codigo,
                Nome = entity.Nome,
                CNPJ = entity.CNPJ,
                Email = entity.Email,
                EnderecoVO = entity.Endereco,
                Telefone = entity.Telefone
            };
            return Task.FromResult(fornecedorRequest);
        }

        public Task MapToEntityAsync(FornecedorRequest dto, Fornecedor entityToUpdate)
        {
            entityToUpdate.Codigo = dto.Codigo;
            entityToUpdate.Nome = dto.Nome;
            entityToUpdate.Email = dto.Email;
            entityToUpdate.Telefone = dto.Telefone;
            entityToUpdate.CNPJ = dto.CNPJ;
            entityToUpdate.Endereco = dto.EnderecoVO;
            return Task.CompletedTask;
        }

        public override Task<Fornecedor> MapToEntityAsync(FornecedorRequest request)
        {
            var fornecedor = new Fornecedor
            {
                Codigo = request.Codigo,
                Nome = request.Nome,
                Email = request.Email,
                Telefone = request.Telefone,
                CNPJ = request.CNPJ,
                Endereco = request.EnderecoVO
            };
            return Task.FromResult(fornecedor);
        }
    }
}
