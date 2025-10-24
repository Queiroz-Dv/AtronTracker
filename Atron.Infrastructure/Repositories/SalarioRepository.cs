using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Atron.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Tracker.Infrastructure.Repositories
{
    public class SalarioRepository : Repository<Salario>, ISalarioRepository
    {
        private AtronDbContext _context;

        public SalarioRepository(AtronDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> AtualizarSalarioRepositoryAsync(int id, Salario salario)
        {
            var entidade = await _context.Salarios.FirstOrDefaultAsync(slr => slr.Id == id);

            entidade.SalarioMensal = salario.SalarioMensal;
            entidade.Ano = salario.Ano;
            entidade.MesId = salario.MesId;           

            try
            {
                _context.Salarios.Update(entidade);
                var atualizado  = await _context.SaveChangesAsync();
                return atualizado > 0;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<bool> CriarSalarioAsync(Salario entidade)
        {
            try
            {
                await _context.Salarios.AddAsync(entidade);
                var gravado = await _context.SaveChangesAsync();
                return gravado > 0;
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