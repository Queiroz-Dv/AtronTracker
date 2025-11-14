using AtronStock.Domain.Enums;
using AtronStock.Domain.ValueObjects;
using System.ComponentModel;

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

        [DisplayName("Endereço")]
        public EnderecoVO EnderecoVO { get; set; }

        public ClienteRequest()
        {
            StatusPessoa = EStatusPessoa.Ativo;
            EnderecoVO = new EnderecoVO();
        }
    }
}