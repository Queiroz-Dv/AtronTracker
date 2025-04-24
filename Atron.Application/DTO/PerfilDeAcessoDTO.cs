using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Atron.Application.DTO
{
    public class PerfilDeAcessoDTO
    {
        public int Id { get; set; }
     
        public string Codigo { get; set; }

        public string Descricao { get; set; }

        public ICollection<ModuloDTO> Modulos { get; set; }  
    }
}
