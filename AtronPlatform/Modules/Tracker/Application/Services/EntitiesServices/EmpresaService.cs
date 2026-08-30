using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTO;
using Application.Interfaces.Services;
using Application.UseCases.EmpresaCases;
using Shared.Domain.ValueObjects;

namespace Application.Services.EntitiesServices
{
    public sealed class EmpresaService(
        CriarEmpresaCase criar,
        ObterEmpresaCase obter,
        AtualizarEmpresaCase atualizar,
        ExcluirEmpresaCase excluir) : IEmpresaService
    {
        public Task<Resultado<EmpresaDTO>> CriarAsync(EmpresaDTO empresa)
            => criar.ExecutarAsync(empresa);

        public Task<Resultado<IReadOnlyList<EmpresaDTO>>> ObterTodosAsync()
            => obter.ObterTodosAsync();

        public Task<Resultado<EmpresaDTO>> ObterPorCodigoAsync(string codigo)
            => obter.ObterPorCodigoAsync(codigo);

        public Task<Resultado<EmpresaDTO>> AtualizarAsync(string codigo, EmpresaDTO empresa)
            => atualizar.ExecutarAsync(codigo, empresa);

        public Task<Resultado> RemoverAsync(string codigo)
            => excluir.ExecutarAsync(codigo);
    }
}
