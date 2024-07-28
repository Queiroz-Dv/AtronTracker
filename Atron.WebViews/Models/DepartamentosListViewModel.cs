using Atron.Application.DTO;
using Shared.DTO;
using System.Collections.Generic;

namespace Atron.WebViews.Models
{
    public class DepartamentosListViewModel
    {
        public IEnumerable<DepartamentoDTO> Departamentos { get; set; }
        public PagingInfo PagingInfo { get; set; }  
    }
}
