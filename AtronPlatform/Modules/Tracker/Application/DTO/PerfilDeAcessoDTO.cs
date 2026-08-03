using System.Collections.Generic;

namespace Application.DTO
{
    public class PerfilDeAcessoDTO
    {
        public PerfilDeAcessoDTO()
        {
            Modulos = new List<ModuloDTO>();
        }
        public int Id { get; set; }

        public string Codigo { get; set; }

        public string Descricao { get; set; }

        public ICollection<ModuloDTO> Modulos { get; set; }
    }
}
