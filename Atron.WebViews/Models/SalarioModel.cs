using Atron.Application.DTO;
using System.Collections.Generic;

namespace Atron.WebViews.Models
{
    public class SalarioModel : DefaultModel<SalarioDTO>
    {
        public SalarioModel()
        {
            Salarios = new List<SalarioDTO>();
            Meses = new List<MesDTO>();
            Usuarios = new List<UsuarioDTO>();
        }

        public UsuarioDTO Usuario { get; set; }
        public MesDTO Mes { get; set; }
        public List<MesDTO> Meses { get; set; }

        public List<UsuarioDTO> Usuarios { get; set; }
        public SalarioDTO Salario { get; set; }
        public List<SalarioDTO> Salarios { get; set; }
    }
}
