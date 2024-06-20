using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Atron.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories
{
    public class DepartamentoRepository : IDepartamentoRepository
    {
        private AtronDbContext _context;

        public DepartamentoRepository(AtronDbContext context)
        {
            _context = context;
        }

        public void AtualizarDepartamentoRepositoryAsync(Departamento departamento)
        {
            throw new NotImplementedException();
        }

        public void CriarDepartamentoRepositoryAsync(Departamento departamento)
        {
            throw new NotImplementedException();
        }

        public Task<Departamento> ObterDepartamentoPorCodigoRepositoryAsync(string codigo)
        {
            throw new NotImplementedException();
        }

        public Task<Departamento> ObterDepartamentoPorIdRepositoryAsync(int? id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Departamento>> ObterDepartmentosAsync()
        {
            throw new NotImplementedException();
        }

        public void RemoverDepartmentoRepositoryAsync(Departamento departamento)
        {
            throw new NotImplementedException();
        }
    }
}
