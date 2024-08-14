using Atron.Application.DTO;
using System.Collections.Generic;

namespace Atron.WebViews.Models
{
    public class UsuarioModel : DefaultModel
    {
        public UsuarioModel()
        {
            Cargos = new List<CargoDTO>();
            Departamentos = new List<DepartamentoDTO>();
            Usuarios= new List<UsuarioDTO>();
        }

        public CargoDTO Cargo { get; set; }

        public DepartamentoDTO Departamento { get; set; }

        public List<CargoDTO> Cargos { get; set; }

        public List<DepartamentoDTO> Departamentos { get; set; }

        public UsuarioDTO Usuario { get; set; }

        public List<UsuarioDTO> Usuarios { get; set; }
    }
}
