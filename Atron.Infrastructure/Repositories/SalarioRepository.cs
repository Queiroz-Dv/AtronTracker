using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Atron.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories
{
    public class SalarioRepository : Repository<Salario>, ISalarioRepository
    {
        private AtronDbContext _context;

        public SalarioRepository(AtronDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task AtualizarSalarioRepositoryAsync(Salario salario)
        {
            var entidade = await _context.Salarios.FirstOrDefaultAsync(slr => slr.Id == salario.Id);

            entidade.SalarioMensal = salario.SalarioMensal;
            entidade.Ano = salario.Ano;            
            entidade.MesId = salario.MesId;
            entidade.UsuarioId = salario.UsuarioId;
            entidade.UsuarioCodigo = salario.UsuarioCodigo;

            try
            {
                _context.Salarios.Update(entidade);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task CriarSalarioAsync(Salario entidade)
        {
            try
            {
                await _context.Salarios.AddAsync(entidade);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<Salario> ObterSalarioPorCodigoUsuario(string codigoUsuario)
        {
            return await _context.Salarios.FirstOrDefaultAsync(slr => slr.UsuarioCodigo == codigoUsuario);
        }

        public Task<Salario> ObterSalarioPorIdAsync(int id)
        {
            return _context.Salarios
                .Include(slr => slr.Usuario)
                //.ThenInclude(crg => crg.Cargo)
               // .Include(dpt => dpt.Usuario.Departamento)
                .FirstOrDefaultAsync(sr => sr.Id == id);
        }

        public Task<List<Salario>> ObterSalariosRepository()
        {
            return _context.Salarios
                .Include(slr => slr.Usuario)
               // .ThenInclude(crg => crg.Cargo)
               // .Include(dpt => dpt.Usuario.Departamento)
                .Include(ms => ms.Mes).ToListAsync();
        }
    }
}