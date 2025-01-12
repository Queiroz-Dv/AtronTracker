using System.Collections.Generic;

namespace Atron.Application.Specifications
{
    public interface ISpecification<T>
    {
        bool IsSatisfiedBy(T entity);
        List<string> Errors { get; }
    }
}