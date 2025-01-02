using Atron.Domain.Entities;
using Atron.Domain.Interfaces.UsuarioInterfaces;
using Atron.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories
{
    public class UsuarioCargoDepartamentoRepository : IUsuarioCargoDepartamentoRepository
    {
        private readonly AtronDbContext _context;

        public UsuarioCargoDepartamentoRepository(AtronDbContext context)
        {
            _context = context;
        }

        public async Task<bool> GravarAssociacaoUsuarioCargoDepartamento(Usuario usuario, Cargo cargo, Departamento departamento)
        {
            var usuarioBd = _context.Usuarios.FirstAsync(usr => usr.Id == usuario.Id);

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
    }
}