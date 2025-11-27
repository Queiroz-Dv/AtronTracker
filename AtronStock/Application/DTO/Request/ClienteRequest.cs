using AtronStock.Domain.Enums;
using AtronStock.Domain.ValueObjects;
using Shared.Domain.ValueObjects;
using System.ComponentModel;

namespace AtronStock.Application.DTO.Request
{
    public class ClienteRequest
    {        
        public string Codigo { get; set; }
        public string Nome { get; set; }        
        public Documento Documento { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public EStatus StatusPessoa { get; set; }

        [DisplayName("Endereço")]
        public EnderecoVO EnderecoVO { get; set; }

        public ClienteRequest()
        {
            StatusPessoa = EStatus.Ativo;
            EnderecoVO = new EnderecoVO();
        }
    }
}