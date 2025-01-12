using Atron.Application.DTO;
using System.Collections.Generic;

namespace Atron.Application.Specifications.DepartamentoSpecifications
{
    public class DepartamentoSpecification : ISpecification<DepartamentoDTO>
    {
        private readonly string _codigo;

        public DepartamentoSpecification(string codigo)
        {
            _codigo = codigo;
        }

        public List<string> Errors => throw new System.NotImplementedException();

        public bool IsSatisfiedBy(DepartamentoDTO entity)
        {
            return entity.Codigo.Equals(_codigo);
        }
    }
}