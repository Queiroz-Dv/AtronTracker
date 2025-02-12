using Atron.Application.DTO;
using System.Collections.Generic;

namespace Atron.WebViews.Models
{
    public class SalarioModel : DefaultModel<SalarioDTO>
    {
        public SalarioModel()
        {          
           Entities = new List<SalarioDTO>();
        }

        public UsuarioDTO Usuario { get; set; }
        public SalarioDTO Salario { get; set; }
    }
}