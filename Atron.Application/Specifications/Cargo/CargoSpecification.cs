using Atron.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atron.Application.Specifications.Cargo
{
    public class CargoSpecification : ISpecification<CargoDTO>
    {
        private readonly string _codigo;
        private readonly string _codigoDepartamento;

        public CargoSpecification(string codigo, string codigoDepartamento)
        {
            _codigo = codigo;
            _codigoDepartamento = codigoDepartamento;
        }
    
        public bool IsSatisfiedBy(CargoDTO entity)
        {
            return entity.Codigo.Equals(_codigo) && entity.DepartamentoCodigo.Equals(_codigoDepartamento);
        }
    }
}