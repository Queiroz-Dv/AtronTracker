using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTO;
using Application.Mapping;
using Domain.Interfaces;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;

namespace Application.UseCases.EmpresaCases
{
    public sealed class ObterEmpresaCase(EmpresaMapping mapping, IEmpresaRepository repository)
    {
        public async Task<Resultado<IReadOnlyList<EmpresaDTO>>> ObterTodosAsync()
        {
            var empresas = await repository.ObterTodosAsync();
            return Resultado<IReadOnlyList<EmpresaDTO>>
                .Sucesso(mapping.MapToDtos(empresas).ToArray());
        }

        public async Task<Resultado<EmpresaDTO>> ObterPorCodigoAsync(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return Resultado<EmpresaDTO>.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var codigoNormalizado = EmpresaMapping.NormalizarCodigo(codigo);
            var empresa = await repository.ObterPorCodigoAsync(codigoNormalizado);
            return empresa is null
                ? Resultado<EmpresaDTO>.Falha(string.Format(
                    NotificacoesPadronizadas.Erro_RegistroComDescricaoNaoEncontrado,
                    codigoNormalizado))
                : Resultado<EmpresaDTO>.Sucesso(mapping.MapToDto(empresa));
        }
    }
}
