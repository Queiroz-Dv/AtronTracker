using AtronStock.Application.DTO.Request;
using AtronStock.Domain.Entities;
using Shared.Application.Interfaces.Mapping;

namespace AtronStock.Application.Mapping
{
    public class FornecedorMapping : Mapper<Fornecedor, FornecedorRequest>
    {
        public override FornecedorRequest MapToDto(Fornecedor entity)
        {
            return new FornecedorRequest
            {
                Codigo = entity.Codigo,
                Nome = entity.Nome,
                CNPJ = entity.CNPJ,
                Email = entity.Email,
                EnderecoVO = entity.Endereco,
                Telefone = entity.Telefone
            };
        }

        public void MapToEntity(FornecedorRequest dto, Fornecedor entityToUpdate)
        {
            entityToUpdate.Codigo = dto.Codigo;
            entityToUpdate.Nome = dto.Nome;
            entityToUpdate.Email = dto.Email;
            entityToUpdate.Telefone = dto.Telefone;
            entityToUpdate.CNPJ = dto.CNPJ;
            entityToUpdate.Endereco = dto.EnderecoVO;
        }

        public override Fornecedor MapToEntity(FornecedorRequest request)
        {
            return new Fornecedor
            {
                Codigo = request.Codigo,
                Nome = request.Nome,
                Email = request.Email,
                Telefone = request.Telefone,
                CNPJ = request.CNPJ,
                Endereco = request.EnderecoVO
            };
        }
    }
}
