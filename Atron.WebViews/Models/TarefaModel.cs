using Atron.Application.DTO;
using System.Collections.Generic;
using System.ComponentModel;

namespace Atron.WebViews.Models
{
    public class TarefaModel : DefaultModel<TarefaDTO>
    {
        public TarefaModel()
        {
            Entities = new List<TarefaDTO>();
        }

        [DisplayName("Usuário")]
        public UsuarioDTO Usuario { get; set; }
        public TarefaDTO Tarefa { get; set; }
    }
}