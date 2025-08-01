using System.ComponentModel;

namespace Atron.Application.DTO.Response
{
    public class CargoResponse
    {
        public CargoResponse(string codigo,
                             string descricao,
                             string departamentoCodigo,
                             string departamentoDescricao)
        {
            Codigo = codigo;
            Descricao = descricao;
            DepartamentoCodigo = departamentoCodigo;
            DepartamentoDescricao = departamentoDescricao;
        }

        [DisplayName("Código")]
        public string Codigo { get; set; }

        [DisplayName("Descrição")]
        public string Descricao { get; set; }

        [DisplayName("Código do departamento")]
        public string DepartamentoCodigo { get; set; }

        [DisplayName("Descrição do departamento")]
        public string DepartamentoDescricao { get; set; }
    }
}