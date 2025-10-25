using Application.DTO;
using System.Collections.Generic;

namespace Atron.WebViews.Models.ControleDeAcesso
{
    public class ModuloModel : DefaultModel<ModuloDTO>
    {
        public ModuloModel()
        {
            Entities = new List<ModuloDTO>();
        }

        public ModuloDTO Modulo { get; set; }
    }
}