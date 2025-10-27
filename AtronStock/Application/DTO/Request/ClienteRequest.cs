using AtronStock.Domain.Enums;

namespace AtronStock.Application.DTO.Request
{
    public class ClienteRequest
    {
        public string Codigo { get; set; }
        public string Nome { get; set; }
        public string CPF { get; set; }
        public string CNPJ { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public EStatusPessoa StatusPessoa { get; set; }
    }
}