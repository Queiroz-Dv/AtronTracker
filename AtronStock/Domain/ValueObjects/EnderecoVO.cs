using System.ComponentModel.DataAnnotations;

namespace AtronStock.Domain.ValueObjects
{
    public class EnderecoVO
    {
        [MaxLength(100)] public string Logradouro { get; set; } = string.Empty;
        [MaxLength(10)] public string Numero { get; set; } = string.Empty;
        [MaxLength(50)] public string Cidade { get; set; } = string.Empty;
        [MaxLength(2)] public string UF { get; set; } = string.Empty;
        [MaxLength(9)] public string CEP { get; set; } = string.Empty;
    }

}