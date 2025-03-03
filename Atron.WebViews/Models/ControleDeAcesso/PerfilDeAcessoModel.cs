using Atron.Application.DTO;
using System.Collections.Generic;

namespace Atron.WebViews.Models.ControleDeAcesso
{
    public class PerfilDeAcessoModel : DefaultModel<PerfilDeAcessoDTO>
    {
        public PerfilDeAcessoModel()
        {
            Entities = new List<PerfilDeAcessoDTO>();
        }

        public PerfilDeAcessoDTO PerfilDeAcesso { get; set; }
    }
}