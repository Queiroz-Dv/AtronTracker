using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Atron.Application.DTO
{
    public class UsuarioDTO : FactoryDTO
    {        
        [JsonIgnore]
        [SwaggerSchema(ReadOnly = true, WriteOnly = true)]
        public int Id { get; set; }        
        public string Nome { get; set; }
        public string Sobrenome { get; set; }
        public DateTime? DataNascimento { get; set; }

        [DisplayName("Salário")]
        public int? Salario { get; set; }
        public string CargoCodigo { get; set; }
        public string DepartamentoCodigo { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Senha { get; set; }

        [JsonIgnore]
        [SwaggerSchema(ReadOnly = true, WriteOnly = true)]
        public int DepartamentoId { get; set; }
        [JsonIgnore]
        [SwaggerSchema(ReadOnly = true, WriteOnly = true)]
        public int CargoId { get; set; }

        public DepartamentoDTO Departamento { get; set; }

        public CargoDTO Cargo { get; set; }

        public List<DepartamentoDTO> Departamentos { get; set; }

        public List<CargoDTO> Cargos { get; set; }

        public bool AssociarCargoDepartamento { get; set; } = false;

        public string NomeCompleto()
        {
            return $"{Nome} {Sobrenome}";
        }
    }
}