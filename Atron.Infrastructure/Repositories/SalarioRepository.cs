using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Atron.Infrastructure.Context;
using Atron.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories
{
    public class SalarioRepository : Repository<Salario>, ISalarioRepository
    {
        //private AtronDbContext _context;

        public SalarioRepository(AtronDbContext context, ILiteDbContext liteDbContext) : base(context, liteDbContext)
        {
            //_context = context;
        }

        public async Task AtualizarSalarioRepositoryAsync(int id, Salario salario)
        {
            var entidade = await _context.Salarios.FirstOrDefaultAsync(slr => slr.Id == id);

            entidade.SalarioMensal = salario.SalarioMensal;
            entidade.Ano = salario.Ano;
            entidade.MesId = salario.MesId;           

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
                .ThenInclude(rel => rel.UsuarioCargoDepartamentos)
                .ThenInclude(crg => crg.Cargo)
                .ThenInclude(dpt => dpt.Departamento)
                .FirstOrDefaultAsync(sr => sr.Id == id);
        }

        public Task<Salario> ObterSalarioPorUsuario(int usuarioId, string usuarioCodigo)
        {
            return _context.Salarios.FirstOrDefaultAsync(slr => slr.UsuarioId == usuarioId && slr.UsuarioCodigo == usuarioCodigo);
        }

        public Task<List<Salario>> ObterSalariosRepository()
        {
            return _context.Salarios
                .Include(slr => slr.Usuario)
                .ThenInclude(rel => rel.UsuarioCargoDepartamentos)
                .ThenInclude(crg => crg.Cargo)
                .ThenInclude(dpt => dpt.Departamento)
                .ToListAsync();
        }
    }
}