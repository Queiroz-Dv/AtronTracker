using Application.DTO;
using System.Collections.Generic;

namespace Atron.WebViews.Models
{
    public class DepartamentoModel : DefaultModel<DepartamentoDTO>
    {
        public DepartamentoModel()
        {
            Entities = new List<DepartamentoDTO>();
        }

        public DepartamentoDTO Departamento { get; set; }
    }
}