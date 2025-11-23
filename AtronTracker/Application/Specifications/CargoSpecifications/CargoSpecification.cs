using Application.DTO;

namespace Application.Specifications.CargoSpecifications
{
    public class CargoSpecification : ISpecification<CargoDTO>
    {
        private readonly string _codigo;
        private readonly string _codigoDepartamento;

        public CargoSpecification(string codigo, string codigoDepartamento)
        {
            _codigo = codigo.ToUpper();
            _codigoDepartamento = codigoDepartamento.ToUpper();
        }

        public bool IsSatisfiedBy(CargoDTO entity)
        {
            return entity.Codigo.ToUpper().Equals(_codigo) &&
                   entity.DepartamentoCodigo.ToUpper().Equals(_codigoDepartamento);
        }
    }
}