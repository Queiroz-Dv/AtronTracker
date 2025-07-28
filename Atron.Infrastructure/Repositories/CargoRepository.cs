using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Atron.Infrastructure.Interfaces;
using Shared.Interfaces.Accessor;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories
{
    public class CargoRepository : ICargoRepository
    {
        private ILiteDbContext context;
        private ILiteUnitOfWork unitOfWork;
        private IServiceAccessor serviceAccessor;

        public CargoRepository(ILiteDbContext context, ILiteUnitOfWork unitOfWork, IServiceAccessor serviceAccessor)
        {
            this.context = context;
            this.unitOfWork = unitOfWork;
            this.serviceAccessor = serviceAccessor;
        }

        public async Task<bool> CriarCargoAsync(Cargo cargo)
        {
            try
            {
                unitOfWork.BeginTransaction();
                var gravado = await context.Cargos.InsertAsync(cargo);
                unitOfWork.Commit();
                return gravado > 0;
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                serviceAccessor.ObterService<MessageModel>().AdicionarErro(ex.Message);
                return false;
            }
        }

        public async Task<Cargo> ObterCargoPorIdAsync(int? id)
        {
            var departamentos = await context.Departamentos.FindAllAsync();
            var cargo = await context.Cargos.FindByIdAsync(id);

            var departamento = (from dpt in departamentos.ToList()
                                where dpt.Id == cargo.DepartamentoId
                                select new Departamento()
                                {
                                    Id = dpt.Id,
                                    Codigo = dpt.Codigo,
                                    Descricao = dpt.Descricao
                                }).FirstOrDefault();

            cargo.Departamento = departamento;

            return cargo;
        }

        public async Task<Cargo> ObterCargoComDepartamentoPorIdAsync(int? id)
        {
            var cargosDb = await context.Cargos.FindAllAsync();
            var departamentosDb = await context.Departamentos.FindAllAsync();

            var cargos = (from pst in cargosDb.ToList()
                          join dept in departamentosDb.ToList() on pst.DepartamentoId equals dept.Id
                          where dept.Id == id
                          select new Cargo
                          {
                              Descricao = pst.Descricao,
                              Departamento = dept,
                              DepartamentoId = dept.Id
                          }).FirstOrDefault();
            return cargos;
        }

        public async Task<IEnumerable<Cargo>> ObterCargosAsync()
        {
            var cargos = await context.Cargos.FindAllAsync();
            var departamentos = await context.Departamentos.FindAllAsync();

            cargos = (from crg in cargos.ToList()
                      join dpt in departamentos.ToList() on crg.DepartamentoId equals dpt.Id
                      select new Cargo
                      {
                          Codigo = crg.Codigo,
                          Descricao = crg.Descricao,
                          DepartamentoId = dpt.Id,
                          DepartamentoCodigo = dpt.Codigo,
                          Departamento = dpt
                      }).ToList();
            return cargos;
        }

        public async Task<bool> RemoverCargoAsync(Cargo cargo)
        {
            try
            {
                unitOfWork.BeginTransaction();
                var cargoBd = await context.Cargos.FindOneAsync(crg => crg.Codigo == cargo.Codigo);
                var deletado = await context.Cargos.DeleteAsync(cargo.Id);
                unitOfWork.Commit();
                return deletado;
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                serviceAccessor.ObterService<MessageModel>().AdicionarErro(ex.Message);
                return false;
            }
        }

        public async Task<bool> AtualizarCargoAsync(Cargo cargo)
        {
            try
            {
                unitOfWork.BeginTransaction();
                var cargoBd = await ObterCargoPorCodigoAsync(cargo.Codigo);
                cargoBd.Descricao = cargo.Descricao;
                cargoBd.DepartamentoId = cargo.DepartamentoId;
                cargoBd.DepartamentoCodigo = cargo.DepartamentoCodigo;

                var atualizado = await context.Cargos.UpdateAsync(cargoBd);
                unitOfWork.Commit();
                return atualizado;
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                serviceAccessor.ObterService<MessageModel>().AdicionarErro(ex.Message);
                return false;
            }
        }

        public async Task<Cargo> ObterCargoPorCodigoAsync(string codigo)
        {
            var departamentos = await context.Departamentos.FindAllAsync();
            var cargo = await context.Cargos.FindOneAsync(crg => crg.Codigo == codigo);

            if (cargo is not null)
            {
                var departamento = (from dpt in departamentos.ToList()
                                    where dpt.Id == cargo.DepartamentoId
                                    select new Departamento()
                                    {
                                        Id = dpt.Id,
                                        Codigo = dpt.Codigo,
                                        Descricao = dpt.Descricao
                                    }).FirstOrDefault();

                cargo.DepartamentoId = departamento.Id;
                cargo.DepartamentoCodigo = departamento.Codigo;
                cargo.Departamento = departamento;
            }

            return cargo;
        }

        public async Task<bool> CargoExiste(string codigo)
        {
            return await context.Cargos.AnyAsync(crg => crg.Codigo == codigo);
        }

        public async Task<IEnumerable<Cargo>> ObterCargosPorDepartamento(int departamentoId, string departamentoCodigo)
        {
            var cargos = await context.Cargos.FindAllAsync();
            var departamentos = await context.Departamentos.FindAllAsync();

            cargos = (from crg in cargos.ToList()
                      join dpt in departamentos.ToList() on crg.DepartamentoId equals dpt.Id
                      where crg.DepartamentoId == departamentoId && crg.DepartamentoCodigo == departamentoCodigo
                      select new Cargo
                      {
                          Codigo = crg.Codigo,
                          Descricao = crg.Descricao,
                          DepartamentoId = dpt.Id,
                          DepartamentoCodigo = dpt.Codigo,
                          Departamento = dpt
                      }).ToList();

            return cargos;
        }
    }
}