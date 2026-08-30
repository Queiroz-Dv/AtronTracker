using Application.DTO;

namespace Application.Specifications.DepartamentoSpecifications
{
    public class DepartamentoSpecification : ISpecification<DepartamentoDTO>
    {
        private readonly string _codigo;

        public DepartamentoSpecification(string codigo)
        {
            _codigo = codigo;
        }

        public bool IsSatisfiedBy(DepartamentoDTO entity)
        {
            return entity.Codigo.Equals(_codigo);
        }
    }
}
