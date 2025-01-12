using Atron.Application.DTO;
using Shared.DTO;
using System.Collections.Generic;
using System.ComponentModel;

namespace Atron.WebViews.Models
{
    public class TarefaModel : DefaultModel
    {
        public TarefaModel()
        {           
            Usuarios = new List<UsuarioDTO>();
            Tarefas = new List<TarefaDTO>();
            PageInfo = new PageInfoDTO();
            Tarefa = new TarefaDTO();
            Usuario = new UsuarioDTO();
        }
    
        [DisplayName("Usuário")]
        public UsuarioDTO Usuario { get; set; }
        public List<UsuarioDTO> Usuarios { get; set; }

        public TarefaDTO Tarefa { get; set; }
        public List<TarefaDTO> Tarefas { get; set; }
    }
}