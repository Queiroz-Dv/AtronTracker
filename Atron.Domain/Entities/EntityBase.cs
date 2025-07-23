using LiteDB;

namespace Atron.Domain.Entities
{
    // Entidade com propriedade padrão de todas as classes
    public abstract class EntityBase
    {
        [BsonId]
        public int Id { get; set; }
    }
}