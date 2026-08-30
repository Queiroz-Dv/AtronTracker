using Application.DTO;
using Application.Mapping;
using Domain.Interfaces;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Threading.Tasks;

namespace Application.UseCases.EmpresaCases
{
    public sealed class AtualizarEmpresaCase(
        EmpresaMapping mapping,
        IEmpresaRepository repository,
        IValidador<EmpresaDTO> validador)
    {
        public async Task<Resultado<EmpresaDTO>> ExecutarAsync(string codigo, EmpresaDTO empresaDTO)
        {
            if (codigo.IsNullOrEmpty())
                return Resultado<EmpresaDTO>.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var erros = validador.Validar(empresaDTO);
            if (erros.TemErros())
                return Resultado<EmpresaDTO>.Falhas(erros);

            var empresa = await repository.ObterPorCodigoAsync(codigo, rastrear: true);
            if (empresa is null)
                return Resultado<EmpresaDTO>.Falha(string.Format(
                    NotificacoesPadronizadas.Erro_RegistroComDescricaoNaoEncontrado,
                    codigo));

            mapping.MapToUpdate(empresaDTO, empresa);
            if (!await repository.AtualizarAsync(empresa))
                return Resultado<EmpresaDTO>.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            return Resultado<EmpresaDTO>
                .Sucesso(mapping.MapToDto(empresa))
                .AdicionarMensagem(string.Format(
                    NotificacoesPadronizadas.Mensagem_RegistroAtualizado,
                    empresa.Codigo));
        }
    }
}
