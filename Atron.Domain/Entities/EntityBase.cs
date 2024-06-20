namespace Atron.Domain.Entities
{
    // Entidade com propriedade padrão de todas as classes
    public abstract class EntityBase
    {
        public int Id { get; protected set; }
    }
}