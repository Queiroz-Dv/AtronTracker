namespace Application.DTO
{
    public class TarefaEstadoDTO
    {
        public int Id { get; set; }
        public string Descricao { get; set; }

        public TarefaEstadoDTO() { }

        public TarefaEstadoDTO(int id, string descricao)
        {
            Id = id;
            Descricao = descricao;
        }
    }
}