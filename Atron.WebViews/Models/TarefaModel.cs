using Atron.Application.DTO;
using System.Collections.Generic;
using System.ComponentModel;

namespace Atron.WebViews.Models
{
    public class TarefaModel : DefaultModel
    {
        public TarefaModel()
        {
            Cargos = new List<CargoDTO>();
            Departamentos = new List<DepartamentoDTO>();
            Usuarios = new List<UsuarioDTO>();
            Tarefas = new List<TarefaDTO>();
        }

        public CargoDTO Cargo { get; set; }

        public List<CargoDTO> Cargos { get; set; }

        public DepartamentoDTO Departamento { get; set; }
        public List<DepartamentoDTO> Departamentos { get; set; }

        [DisplayName("Usuário")]
        public UsuarioDTO Usuario { get; set; }
        public List<UsuarioDTO> Usuarios { get; set; }

        public TarefaDTO Tarefa { get; set; }
        public List<TarefaDTO> Tarefas { get; set; }
    }
}