using Atron.Application.DTO;

namespace Atron.Application.Specifications.Departamento
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