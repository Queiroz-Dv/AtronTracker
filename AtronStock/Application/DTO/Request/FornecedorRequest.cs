using AtronStock.Domain.ValueObjects;

namespace AtronStock.Application.DTO.Request
{
    public class FornecedorRequest
    {
        public string Codigo { get; set; } = string.Empty;

        public string Nome { get; set; } = string.Empty;

        public string Email { get; set; }

        public string Telefone { get; set; }

        public string CNPJ { get; set; }

        public EnderecoVO EnderecoVO { get; set; }
    }
}