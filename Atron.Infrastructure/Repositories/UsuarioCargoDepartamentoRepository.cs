using Atron.Domain.Entities;
using Atron.Domain.Interfaces.UsuarioInterfaces;
using Atron.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories
{
    public class UsuarioCargoDepartamentoRepository : Repository<UsuarioCargoDepartamento>, IUsuarioCargoDepartamentoRepository
    {
        private readonly AtronDbContext _context;

        public UsuarioCargoDepartamentoRepository(AtronDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<UsuarioCargoDepartamento> ObterPorChaveDoUsuario(int usuarioId, string usuarioCodigo)
        {
            return await _context.UsuarioCargoDepartamentos.FirstOrDefaultAsync(rel => rel.UsuarioId == usuarioId && rel.UsuarioCodigo == usuarioCodigo);
        }

        public async Task<bool> GravarAssociacaoUsuarioCargoDepartamento(Usuario usuario, Cargo cargo, Departamento departamento)
        {
            var usuarioBd = await _context.Usuarios.FirstAsync(usr => usr.Codigo == usuario.Codigo);

            var associacao = new UsuarioCargoDepartamento()
            {
                UsuarioId = usuarioBd.Id,
                UsuarioCodigo = usuario.Codigo,

                DepartamentoId = departamento.Id,
                DepartamentoCodigo = departamento.Codigo,

                CargoId = cargo.Id,
                CargoCodigo = cargo.Codigo
            };

            try
            {
                await _context.UsuarioCargoDepartamentos.AddAsync(associacao);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<UsuarioCargoDepartamento>> ObterPorDepartamento(int id, string codigo)
        {
            return await _context.UsuarioCargoDepartamentos.Where(rel => rel.DepartamentoId == id && rel.DepartamentoCodigo == codigo).ToListAsync();
        }
    }
}