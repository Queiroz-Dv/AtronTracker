using System.Collections.Generic;

namespace Application.DTO
{
    public class TarefaEstadoDTO
    {
        public int Id { get; set; }
        public string Descricao { get; set; }

        public static List<TarefaEstadoDTO> TarefasEstados()
        {
            return new List<TarefaEstadoDTO>
                {
                    new TarefaEstadoDTO { Id = 1, Descricao = "Em atividade" },
                    new TarefaEstadoDTO { Id = 2, Descricao = "Pendente de aprovação" },
                    new TarefaEstadoDTO { Id = 3, Descricao = "Entregue" },
                    new TarefaEstadoDTO { Id = 4, Descricao = "Finalizada" },
                    new TarefaEstadoDTO { Id = 5, Descricao = "Iniciada" }
                };
        }
    }
}