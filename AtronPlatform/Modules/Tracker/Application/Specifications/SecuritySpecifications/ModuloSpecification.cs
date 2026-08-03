using Application.DTO;

namespace Application.Specifications.SecuritySpecifications
{
    public class ModuloSpecification : ISpecification<ModuloDTO>
    {
        public readonly string _codigo;

        public ModuloSpecification(string codigo)
        {
            _codigo = codigo;
        }

        public bool IsSatisfiedBy(ModuloDTO entity)
        {
            return entity.Codigo.ToUpper() == _codigo.ToUpper();
        }
    }
}