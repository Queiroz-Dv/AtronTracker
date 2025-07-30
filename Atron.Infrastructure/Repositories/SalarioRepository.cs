using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Atron.Domain.Interfaces.UsuarioInterfaces;
using Atron.Infrastructure.Interfaces;
using Shared.Interfaces.Accessor;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories
{
    public class SalarioRepository : ISalarioRepository
    {
        private ILiteDbContext context;
        private ILiteUnitOfWork unitOfWork;
        private IServiceAccessor serviceAccessor;
        private readonly IUsuarioRepository usuarioRepository;

        public SalarioRepository(ILiteDbContext context,
                                 ILiteUnitOfWork unitOfWork,
                                 IServiceAccessor serviceAccessor,
                                 IUsuarioRepository usuarioRepository)
        {
            this.context = context;
            this.unitOfWork = unitOfWork;
            this.serviceAccessor = serviceAccessor;
            this.usuarioRepository = usuarioRepository;
        }

        public async Task<bool> AtualizarSalarioRepositoryAsync(int id, Salario salario)
        {
            try
            {
                unitOfWork.BeginTransaction();
                var salarioBd = await context.Salarios.FindByIdAsync(id);

                salarioBd.UsuarioId = salario.UsuarioId;
                salarioBd.UsuarioCodigo = salario.UsuarioCodigo;
                salarioBd.SalarioMensal = salario.SalarioMensal;
                salarioBd.Ano = salario.Ano;
                salarioBd.MesId = salario.MesId;
                var atualizado = await context.Salarios.UpdateAsync(salarioBd);
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

        public async Task<bool> CriarSalarioAsync(Salario entidade)
        {
            try
            {
                unitOfWork.BeginTransaction();
                var criado = await context.Salarios.InsertAsync(entidade);
                unitOfWork.Commit();
                return criado > 0;

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                serviceAccessor.ObterService<MessageModel>().AdicionarErro(ex.Message);
                return false;
            }
        }

        public async Task<Salario> ObterSalarioPorCodigoUsuario(string codigoUsuario)
        {
            return await context.Salarios.FindOneAsync(slr => slr.UsuarioCodigo == codigoUsuario);
        }

        public async Task<Salario> ObterSalarioPorIdAsync(int id)
        {
            var salario = await context.Salarios.FindByIdAsync(id);
            var usuario = await usuarioRepository.ObterUsuarioPorCodigoAsync(salario.UsuarioCodigo);

            return new Salario
            {
                Id = salario.Id,
                UsuarioId = salario.UsuarioId,
                UsuarioCodigo = salario.UsuarioCodigo,
                SalarioMensal = salario.SalarioMensal,
                Ano = salario.Ano,
                MesId = salario.MesId,
                Usuario = usuario
            };
        }

        public async Task<Salario> ObterSalarioPorUsuario(int usuarioId, string usuarioCodigo)
        {
            var salario = await context.Salarios.FindOneAsync(slr => slr.UsuarioId == usuarioId && slr.UsuarioCodigo == usuarioCodigo);
            var usuario = await usuarioRepository.ObterUsuarioPorCodigoAsync(salario.UsuarioCodigo);

            return new Salario
            {
                Id = salario.Id,
                UsuarioId = salario.UsuarioId,
                UsuarioCodigo = salario.UsuarioCodigo,
                SalarioMensal = salario.SalarioMensal,
                Ano = salario.Ano,
                MesId = salario.MesId,
                Usuario = usuario
            };
        }

        public async Task<List<Salario>> ObterSalariosRepository()
        {
            var salarios = await context.Salarios.FindAllAsync();
            var usuarios = await usuarioRepository.ObterTodosUsuariosDoIdentity();

            var salariosComUsuarios = salarios.Select(salario => new Salario
            {
                Id = salario.Id,
                UsuarioId = salario.UsuarioId,
                UsuarioCodigo = salario.UsuarioCodigo,
                SalarioMensal = salario.SalarioMensal,
                Ano = salario.Ano,
                MesId = salario.MesId,
                Usuario = usuarios.FirstOrDefault(u => u.Codigo == salario.UsuarioCodigo)
            }).ToList();

            return salariosComUsuarios;
        }

        public async Task<bool> RemoverInformacaoDeSalarioPorId(int id)
        {
            try
            {
                var salario = await context.Salarios.FindByIdAsync(id);
                if (salario != null)
                {
                    unitOfWork.BeginTransaction();
                    var removido = await context.Salarios.DeleteAsync(salario.Id);
                    unitOfWork.Commit();
                    return removido;
                }

                serviceAccessor.ObterService<MessageModel>().AdicionarErro("Salário não encontrado.");
                return false;
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                serviceAccessor.ObterService<MessageModel>().AdicionarErro(ex.Message);
                return false;
            }
        }

        public async Task<string> ObterDescricaoDoMes(int mesId)
        {
            var mes = await context.Meses.FindByIdAsync(mesId);
            return mes?.Descricao;
        }
    }
}