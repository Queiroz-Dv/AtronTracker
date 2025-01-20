using Atron.Application.DTO;
using Shared.DTO;
using System.Collections.Generic;

namespace Atron.WebViews.Models
{
    public class DepartamentoModel : DefaultModel<DepartamentoDTO>
    {        
        public DepartamentoDTO Departamento { get; set; }  

        public IEnumerable<DepartamentoDTO> Departamentos { get; set; }
    }
}
