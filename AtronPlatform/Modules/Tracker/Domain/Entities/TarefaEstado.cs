namespace Domain.Entities
{
    public class TarefaEstado
    {
        public int Id { get; set; }
        public string Descricao { get; set; }

        public TarefaEstado() { }

        public TarefaEstado(int id, string descricao)
        {
            Id = id;
            Descricao = descricao;
        }
    }
}