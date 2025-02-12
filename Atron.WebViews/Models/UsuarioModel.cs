using Atron.Application.DTO;
using System.Collections.Generic;

namespace Atron.WebViews.Models
{
    public class UsuarioModel : DefaultModel<UsuarioDTO>
    {
        public UsuarioModel()
        {          
            Entities = new List<UsuarioDTO>();
        }

        public UsuarioDTO Usuario { get; set; }

        public CargoDTO Cargo { get; set; }

        public DepartamentoDTO Departamento { get; set; }
    }
}
