using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public sealed class Cliente
    {
        [Key] public int Id { get; set; }

        [MaxLength(25), Required]
        public string Codigo { get; set; }

        [MaxLength(50), Required]
        public string Nome { get; set; }

        [MaxLength(11)]
        public string CPF { get; set; }

        [MaxLength(14)]
        public string CNPJ { get; set; }

        public StatusPessoa Status { get; set; }

        public Endereco Endereco { get; set; }

        [MaxLength(50), Required]
        public string Email { get; set; }

        [MaxLength(15), Required]
        public string Telefone { get; set; }

        public List<Venda> Vendas { get; set; }
    }

    public enum StatusPessoa
    {
        [Description("Ativo")]
        Ativo = 1,
        [Description("Inativo")]
        Inativo = 2,
        [Description("Removido")]
        Removido = 3
    }

    public class Endereco
    {
        [MaxLength(100)] public string Logradouro { get; set; }
        [MaxLength(10)] public string Numero { get; set; }
        [MaxLength(50)] public string Cidade { get; set; }
        [MaxLength(2)] public string UF { get; set; }
        [MaxLength(9)] public string CEP { get; set; }
    }

}