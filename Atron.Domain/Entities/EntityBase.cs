using System;

namespace Atron.Domain.Entities
{
    // Entidade com propriedade padrão de todas as classes
    public abstract class EntityBase
    {
        public int Id { get; protected set; }

        public Guid IdSequencial { get; protected set; }

        public virtual void SetId(int id)
        {
            Id = id;
        }
    }
}