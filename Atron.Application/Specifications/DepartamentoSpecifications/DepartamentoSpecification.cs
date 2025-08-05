using Atron.Application.DTO;

namespace Atron.Application.Specifications.DepartamentoSpecifications
{
    public class DepartamentoSpecification : ISpecification<DepartamentoDTO>
    {
        private readonly string _codigo;

        public DepartamentoSpecification(string codigo)
        {
            _codigo = codigo.ToUpper();
        }

        public bool IsSatisfiedBy(DepartamentoDTO entity)
        {
            return entity.Codigo.ToUpper().Equals(_codigo);
        }
    }
}