using Atron.Application.DTO;

namespace Atron.Application.Specifications.SecuritySpecifications
{
    public class ModuloExistenteSpecification : ISpecification<ModuloDTO>
    {
        private readonly ModuloDTO _moduloExistente;

        public ModuloExistenteSpecification(ModuloDTO moduloExistente)
        {
            _moduloExistente = moduloExistente;
        }

        public bool IsSatisfiedBy(ModuloDTO dto)
        {
            return _moduloExistente is not null && _moduloExistente.Codigo == dto.Codigo.ToUpper();
        }
    }
}