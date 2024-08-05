namespace Atron.Domain.Entities
{
    public sealed class Departamento : EntityBase
    {
        public string Codigo { get; private set; }
        public string Descricao { get; private set; }

        public void AtualizarDescricao(string descricao)
        {
            Descricao = descricao;
        }
    }
}