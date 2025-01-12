using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Atron.Application.DTO
{
    public class SalarioDTO : FactoryDTO
    {        
        public int Id { get; set; }

        [JsonIgnore]
        [SwaggerSchema(ReadOnly = true, WriteOnly = true)]
        public int UsuarioId { get; set; }
        public string UsuarioCodigo { get; set; }

        public UsuarioDTO Usuario { get; set; }

        public int MesId { get; set; }

        public MesDTO Mes { get; set; }

        public string? Ano { get; set; }

        [DisplayName("Salário Atual")]
        public int SalarioMensal { get; set; }
    }
}