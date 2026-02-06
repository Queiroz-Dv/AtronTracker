using AtronTracker.Infrastructure.Context;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    /// <summary>
    /// Repository para operações de persistência de Cargo
    /// </summary>
    public class CargoRepository : Repository<Cargo>, ICargoRepository
    {
        private AtronDbContext _context;

        public CargoRepository(AtronDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Cria um novo cargo no banco de dados
        /// </summary>
        /// <param name="cargo">Entidade Cargo a ser criada</param>
        /// <returns>True se a operação foi bem sucedida</returns>
        public async Task<bool> CriarCargoAsync(Cargo cargo)
        {
            await _context.Cargos.AddAsync(cargo);
            var cargoGravado = await _context.SaveChangesAsync();
            return cargoGravado > 0;
        }

        /// <summary>
        /// Obtém um cargo pelo ID
        /// </summary>
        /// <param name="id">Identificador do cargo</param>
        /// <returns>Cargo encontrado ou null</returns>
        public async Task<Cargo> ObterCargoPorIdAsync(int? id)
        {
            var cargo = await _context.Cargos.FindAsync(id);
            return cargo;
        }

        /// <summary>
        /// Obtém um cargo com seu departamento pelo ID
        /// </summary>
        /// <param name="id">Identificador do cargo</param>
        /// <returns>Cargo com departamento ou null</returns>
        public async Task<Cargo> ObterCargoComDepartamentoPorIdAsync(int? id)
        {
            var cargos = await (from pst in _context.Cargos
                                join dept in _context.Departamentos on pst.DepartamentoId equals dept.Id
                                where dept.Id == id
                                select new Cargo
                                {
                                    Descricao = pst.Descricao,
                                    Departamento = dept,
                                    DepartamentoId = dept.Id
                                }).FirstOrDefaultAsync();

            return cargos;
        }

        /// <summary>
        /// Obtém todos os cargos com seus departamentos
        /// </summary>
        /// <returns>Lista de cargos</returns>
        public async Task<IEnumerable<Cargo>> ObterCargosAsync()
        {
            var cargos = await _context.Cargos
                .Include(dpt => dpt.Departamento)
                .AsNoTracking()
                .OrderByDescending(c => c.Codigo)
                .ToListAsync();
            return cargos;
        }

        /// <summary>
        /// Remove um cargo do banco de dados
        /// </summary>
        /// <param name="cargo">Entidade Cargo a ser removida</param>
        /// <returns>True se a operação foi bem sucedida</returns>
        public async Task<bool> RemoverCargoAsync(Cargo cargo)
        {
            _context.Cargos.Remove(cargo);
            var cargoRemovido = await _context.SaveChangesAsync();
            return cargoRemovido > 0;
        }

        /// <summary>
        /// Atualiza um cargo existente no banco de dados
        /// </summary>
        /// <param name="cargo">Entidade Cargo com os dados atualizados</param>
        /// <returns>True se a operação foi bem sucedida</returns>
        public async Task<bool> AtualizarCargoAsync(Cargo cargo)
        {
            var atualizado = await _context.SaveChangesAsync();
            return atualizado > 0;
        }

        /// <summary>
        /// Obtém um cargo pelo código
        /// </summary>
        /// <param name="codigo">Código do cargo</param>
        /// <returns>Cargo encontrado ou null</returns>
        public async Task<Cargo> ObterCargoPorCodigoAsync(string codigo)
        {
            return await _context.Cargos
                .Include(dpt => dpt.Departamento)
                .FirstOrDefaultAsync(crg => crg.Codigo == codigo);
        }       

        /// <summary>
        /// Obtém cargos por departamento
        /// </summary>
        /// <param name="departamentoId">ID do departamento</param>
        /// <param name="departamentoCodigo">Código do departamento</param>
        /// <returns>Lista de cargos do departamento</returns>
        public async Task<IEnumerable<Cargo>> ObterCargosPorDepartamento(int departamentoId, string departamentoCodigo)
        {
            return await _context.Cargos
                .Where(crg => crg.DepartamentoId == departamentoId && crg.DepartamentoCodigo == departamentoCodigo)
                .ToListAsync();
        }
    }
}