using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Atron.Application.DTO
{
    public class DepartamentoDTO
    {
        public DepartamentoDTO() { }

        public DepartamentoDTO(string codigo, string descricao)
        {
            Codigo = codigo.ToUpper();
            Descricao = descricao.ToUpper();
        }

        public int Id { get; set; }

        public string Codigo { get; set; }

        public string Descricao { get; set; }

        public string ObterCodigoComDescricao()
        {
            return $"{Codigo} - {Descricao}";
        }
    }
}