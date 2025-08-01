using Atron.Domain.Entities;
using Atron.Domain.Interfaces.UsuarioInterfaces;
using Atron.Infrastructure.Context;
using Atron.Infrastructure.Interfaces;
using Shared.Interfaces.Accessor;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories
{
    public class UsuarioCargoDepartamentoRepository : IUsuarioCargoDepartamentoRepository
    {
        private ILiteDbContext context;
        private ILiteUnitOfWork unitOfWork;
        private IServiceAccessor serviceAccessor;

        public UsuarioCargoDepartamentoRepository(ILiteDbContext context, ILiteUnitOfWork unitOfWork, IServiceAccessor serviceAccessor)
        {
            this.context = context;
            this.unitOfWork = unitOfWork;
            this.serviceAccessor = serviceAccessor;
        }

        public async Task<UsuarioCargoDepartamento> ObterPorChaveDoUsuario(int usuarioId, string usuarioCodigo)
        {
            return await context.UsuarioCargoDepartamentos.FindOneAsync(rel => rel.UsuarioId == usuarioId && rel.UsuarioCodigo == usuarioCodigo);
        }

        public async Task<bool> GravarAssociacaoUsuarioCargoDepartamento(Usuario usuario, Cargo cargo, Departamento departamento)
        {
            var usuarioBd = await context.Usuarios.FindOneAsync(usr => usr.Codigo == usuario.Codigo);

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

                unitOfWork.BeginTransaction();
                int gravado = await context.UsuarioCargoDepartamentos.InsertAsync(associacao);
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

        public async Task<IEnumerable<UsuarioCargoDepartamento>> ObterPorDepartamento(int id, string codigo)
        {
            return await context.UsuarioCargoDepartamentos.FindAllAsync(rel => rel.DepartamentoId == id && rel.DepartamentoCodigo == codigo);            
        }

        public async Task<bool> RemoverRelacionamentoPorDepartamentoRepository(UsuarioCargoDepartamento item)
        {
            try
            {
                unitOfWork.BeginTransaction();
                var deletado = await context.UsuarioCargoDepartamentos.DeleteAsync(item.Id);
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

        public async Task<bool> RemoverRelacionamentoPorId(int id)
        {
            try
            {
                unitOfWork.BeginTransaction();
                var deletado = await context.UsuarioCargoDepartamentos.DeleteAsync(id);
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

        public async Task<bool> AtualizarAssociacaoUsuarioCargoDepartamento(Usuario usuario, Cargo cargo, Departamento departamento)
        {
            var associacaoBd = await context .UsuarioCargoDepartamentos.FindOneAsync(rel => rel.UsuarioCodigo == usuario.Codigo &&
                                                                        rel.DepartamentoCodigo == departamento.Codigo &&
                                                                        rel.CargoCodigo == cargo.Codigo);

            associacaoBd.UsuarioId = usuario.Id;
            associacaoBd.UsuarioCodigo = usuario.Codigo;
            associacaoBd.CargoId = cargo.Id;
            associacaoBd.CargoCodigo = cargo.Codigo;
            associacaoBd.DepartamentoId = departamento.Id;
            associacaoBd.DepartamentoCodigo = departamento.Codigo;

            try
            {
                unitOfWork.BeginTransaction();
                var atualizado = await context.UsuarioCargoDepartamentos.UpdateAsync(associacaoBd);
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
    }
}